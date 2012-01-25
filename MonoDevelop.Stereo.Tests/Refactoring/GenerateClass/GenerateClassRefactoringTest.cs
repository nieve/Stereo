using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.GenerateClass;
using Rhino.Mocks;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo;

namespace MonoDevelop.Stereo.Tests
{
	[TestFixture()]
	public class GenerateClassRefactoringTest
	{
		IProvideResolvedTypeNameResult typeNameProvider = MockRepository.GenerateMock<IProvideResolvedTypeNameResult>();
		GenerateClassRefactoring generateClassRefactoring;
		
		[TestFixtureSetUp]
		public void SetUp(){
			generateClassRefactoring = new GenerateClassRefactoring(typeNameProvider);
		}
		
		[Test()]
		public void TestCase ()
		{
			var nmspc = "Foo.Bar";
			var clsName = "NewClass";
			AnonymousType anonymousType = new AnonymousType {Namespace = nmspc};
			MemberResolveResult resolvedResult = new MemberResolveResult(anonymousType);
			resolvedResult.CallingType = anonymousType;
			resolvedResult.ResolvedExpression = new ExpressionResult(clsName);
			typeNameProvider.Stub(p=>p.GetResolvedTypeNameResult()).Return(resolvedResult);
			
			var changes = generateClassRefactoring.PerformChanges(null, null);
			
			Assert.IsNotNull(changes);
			Assert.AreEqual(1, changes.Count);
			Assert.IsInstanceOfType(typeof(CreateFileChange), changes[0]);
			var change = changes[0] as CreateFileChange;
			Assert.AreEqual(clsName + ".cs", change.FileName);
			string contentFormat = "namespace {0} {{\r\n\tpublic class {1}{{\r\n\t\t\r\n\t}}\r\n}}";
			Assert.AreEqual(contentFormat.ToFormat(nmspc,clsName), change.Content);
		}
	}
}

