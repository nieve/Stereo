using System;
using NUnit.Framework;
using MonoDevelop.Stereo.Refactoring.CreateDerivedType;
using Rhino.Mocks;

namespace MonoDevelop.Stereo.Tests.CreateDerivedTypeRefactoringTest
{
	[TestFixture]
	public class IsValid
	{
		CreateDerivedTypeRefactoring subject;
		INonConcreteTypeContext ctx = MockRepository.GenerateStub<INonConcreteTypeContext>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new CreateDerivedTypeRefactoring(ctx);
		}
		
		[Test]
		public void Returns_true_when_current_location_is_non_concrete_type ()
		{
			ctx.Stub(c=>c.IsCurrentLocationNonConcreteType()).Return (true);
			
			Assert.IsTrue(subject.IsValid());
		}
	}
}

