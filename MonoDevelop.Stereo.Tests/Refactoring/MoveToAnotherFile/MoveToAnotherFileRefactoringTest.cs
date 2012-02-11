using System;
using NUnit.Framework;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using Rhino.Mocks;
using MonoDevelop.Stereo.Refactoring;
using System.Collections.Generic;
using MonoDevelop.Projects.Dom;

namespace MonoDevelop.Stereo.MoveToAnotherFileRefactoringTest
{
	[TestFixture]
	public class IsValid
	{
		IParseDocument docParser = MockRepository.GenerateMock<IParseDocument>();
		MoveToAnotherFileRefactoring subject;
		
		[SetUp]
		public void SetUp(){
			subject = new MoveToAnotherFileRefactoring(docParser, null);
		}
		
		[TearDown]
		public void TearDown(){
			docParser = MockRepository.GenerateMock<IParseDocument>();
		} 
		
		[Test]
		public void Validates_when_types_were_found_and_current_location_is_not_type_with_file_name(){
			docParser.Stub(dp=>dp.IsCurrentPositionTypeDeclarationUnmatchingFileName()).Return(true);
			docParser.Stub(dp=>dp.GetTypes()).Return(new List<IType>{new AnonymousType(),new AnonymousType()});
			
			var validation = subject.IsValid(null);
			
			Assert.IsTrue(validation);
		}
		
		[Test]
		public void Invalidates_when_one_type_was_found(){
			docParser.Stub(dp=>dp.IsCurrentPositionTypeDeclarationUnmatchingFileName()).Return(true);
			docParser.Stub(dp=>dp.GetTypes()).Return(new List<IType>{new AnonymousType()});
			
			var validation = subject.IsValid(null);
			
			Assert.IsFalse(validation);
		}
		
		[Test]
		public void Invalidates_when_current_location_is_type_with_file_name(){
			docParser.Stub(dp=>dp.IsCurrentPositionTypeDeclarationUnmatchingFileName()).Return(false);
			docParser.Stub(dp=>dp.GetTypes()).Return(new List<IType>{new AnonymousType(),new AnonymousType()});
			
			var validation = subject.IsValid(null);
			
			Assert.IsFalse(validation);
		}
	}
}

