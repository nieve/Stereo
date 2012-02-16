using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using NUnit.Framework;
using System.Linq;

namespace MonoDevelop.Stereo.Tests.RefactoringTasksProviderTest
{
	[TestFixture]
	public class GetPossibleRefactoring
	{
		RefactoringTasksProvider subject = new RefactoringTasksProvider();
		
		[Test()]
		public void Returns_tasks ()
		{
			var tasks = subject.GetPossibleRefactoring();
			Assert.IsNotNull(tasks);
		}
		
		[Test()]
		public void Returns_two_tasks ()
		{
			var tasks = subject.GetPossibleRefactoring();
			Assert.That(tasks.Count() == 2, "should have returned 2 tasks, but returned " + tasks.Count());
		}
		
		[Test()]
		public void Returns_generate_new_type ()
		{
			var tasks = subject.GetPossibleRefactoring();
			Assert.That(tasks.Any(t=>t.GetType().Name == "GenerateNewTypeRefactoring"), "should have returned GenerateNewTypeRefactoring");
		}
		
		[Test()]
		public void Returns_move_to_another_file ()
		{
			var tasks = subject.GetPossibleRefactoring();
			Assert.That(tasks.Any(t=>t.GetType().Name == "MoveToAnotherFileRefactoring"), "should have returned MoveToAnotherFileRefactoring");
		}
	}
}

