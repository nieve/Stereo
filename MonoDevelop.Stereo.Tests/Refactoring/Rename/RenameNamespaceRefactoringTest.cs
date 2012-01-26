using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.Rename;
using MonoDevelop.Refactoring;
using MonoDevelop.Projects.Dom;
using Rhino.Mocks;
using MonoDevelop.Core;

namespace MonoDevelop.Stereo.RenameNamespaceRefactoringTest
{
	[TestFixture]
	public class IsValid
	{
		RenameNamespaceRefactoring renameNamespaceRefactoring = new RenameNamespaceRefactoring();
		
		[Test()]
		public void Validates_options_with_namespace_resolve_result ()
		{
			var options = new RefactoringOptions{ResolveResult = new NamespaceResolveResult("Namespace")};
			Assert.IsTrue(renameNamespaceRefactoring.IsValid(options));
		}
	}
	
//	[TestFixture]
//	public class PerformChanges
//	{
//		RenameNamespaceRefactoring renameNamespaceRefactoring;
//		IFindNamespaceReference refFinder = MockRepository.GenerateStub<IFindNamespaceReference>();
//		RefactoringOptions options = new RefactoringOptions();
//		
//		[SetUp]
//		public void SetUp(){
//			renameNamespaceRefactoring = new RenameNamespaceRefactoring(refFinder);
//		}
//		
//		[Test()]
//		public void Returns_null_when_found_references_are_null ()
//		{
//			refFinder.Stub(f=>f.FindReferences(Arg<NamespaceResolveResult>.Is.Anything, Arg<IProgressMonitor>.Is.Anything))
//				.Return(null);
//			Assert.IsNull(renameNamespaceRefactoring.PerformChanges(options, null));
//		}
//	}
}

