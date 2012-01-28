using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using Rhino.Mocks;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo;
using MonoDevelop.Core;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.GenerateNewTypeRefactoringTest {
	[TestFixture]
	public class PerformingChanges
	{
		IParseDocument docParser = MockRepository.GenerateMock<IParseDocument>();
		GenerateNewTypeRefactoring generateClassRefactoring;
		string nmspc = "Foo.Bar";
		string clsName = "NewClass";
		string dir = @"c:\some\path\";
		List<Change> changes = null;
		MemberResolveResult resolvedResult;
		
		[TestFixtureSetUp]
		public void SetUp(){
			generateClassRefactoring = new GenerateNewTypeRefactoring(docParser, new NewTypeFileFormatResolver());
		}
		
		[SetUp]
		public void SetTest(){
			AnonymousType anonymousType = new AnonymousType {Namespace = nmspc};
			resolvedResult = new MemberResolveResult(anonymousType);
			resolvedResult.CallingType = anonymousType;
			resolvedResult.ResolvedExpression = new ExpressionResult(clsName);
			docParser.Stub(p=>p.GetResolvedTypeNameResult()).Return(resolvedResult);
			docParser.Stub(p=>p.GetCurrentFilePath()).Return(new FilePath(dir + "current.cs"));
			
			changes = generateClassRefactoring.PerformChanges(null, null);
		}
		
		[Test()]
		public void Returns_a_change ()
		{
			changes = generateClassRefactoring.PerformChanges(null, null);
			
			Assert.IsNotNull(changes);
			Assert.AreEqual(1, changes.Count);
		}
		
		[Test()]
		public void Returns_a_create_file_change ()
		{
			changes = generateClassRefactoring.PerformChanges(null, null);
			Assert.IsInstanceOfType(typeof(CreateFileChange), changes[0]);
		}
		
		[Test()]
		public void Returns_change_with_new_full_file_name ()
		{
			var change = changes[0] as CreateFileChange;
			Assert.AreEqual(dir + clsName + ".cs", change.FileName);
		}
		
		[Test()]
		public void Returns_change_with_new_type_content ()
		{
			var change = changes[0] as CreateFileChange;
			string contentFormat = "namespace {0} {{\r\n\tpublic class {1}{{\r\n\t\t\r\n\t}}\r\n}}";
			Assert.AreEqual(contentFormat.ToFormat(nmspc,clsName), change.Content);
		}
	}

	[TestFixture]
	public class IsValid {
		IParseDocument docParser = MockRepository.GenerateMock<IParseDocument>();
		GenerateNewTypeRefactoring generateClassRefactoring;
		
		[TestFixtureSetUp]
		public void SetUp(){
			generateClassRefactoring = new GenerateNewTypeRefactoring(docParser, null);
		}
		
		[Test()]
		public void Validates_only_expression_that_isnt_member_nor_type ()
		{
			MemberResolveResult result = new MemberResolveResult(null){
				ResolvedMember = null, 
				ResolvedType = new DomReturnType{Type = null},
				ResolvedExpression = new ExpressionResult("SomeClass")
			};
			docParser.Stub(p=>p.GetResolvedTypeNameResult()).Return(result);
			
			Assert.IsTrue(generateClassRefactoring.IsValid(null));
		}
	}
}

