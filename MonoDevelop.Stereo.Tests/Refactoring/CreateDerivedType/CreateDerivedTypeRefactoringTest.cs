using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Stereo.Refactoring.CreateDerivedType;
using MonoDevelop.Stereo.Refactoring.NewTypeFormatProviders;
using NUnit.Framework;
using Rhino.Mocks;

namespace MonoDevelop.Stereo.Tests.CreateDerivedTypeRefactoringTest
{
	[TestFixture]
	public class IsValid
	{
		CreateDerivedTypeRefactoring subject;
		INonConcreteTypeContext ctx = MockRepository.GenerateStub<INonConcreteTypeContext>();
		IResolveTypeContent resolver = MockRepository.GenerateStub<IResolveTypeContent>();
		INameValidator validator = MockRepository.GenerateStub<INameValidator>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new CreateDerivedTypeRefactoring(ctx, resolver, validator);
		}
		
		[Test]
		public void Returns_true_when_current_location_is_non_concrete_type ()
		{
			ctx.Stub(c=>c.IsCurrentLocationNonConcreteType()).Return (true);
			
			Assert.IsTrue(subject.IsValid());
		}
	}
	
	[TestFixture]
	public class Run
	{
		CreateDerivedTypeRefactoring subject;
		INonConcreteTypeContext ctx = MockRepository.GenerateStub<INonConcreteTypeContext>();
		IResolveTypeContent resolver = MockRepository.GenerateStub<IResolveTypeContent>();
		INameValidator validator = MockRepository.GenerateStub<INameValidator>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new CreateDerivedTypeRefactoring(ctx, resolver, validator);
		}
		
		[Test]
		public void Handles_interfaces ()
		{
			
		}
	}
}

