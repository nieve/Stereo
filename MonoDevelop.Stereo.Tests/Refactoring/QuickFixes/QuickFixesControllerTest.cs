using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using Rhino.Mocks;
using NUnit.Framework;
using System.Collections.Generic;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.QuickFixesControllerTest
{
	[TestFixture]
	public class Ctor
	{
		FakeQuickFixDisplayer fixSelection;
		QuickFixesController subject;
		static private IRefactorTask selectedTask = MockRepository.GenerateMock<IRefactorTask>();
		
		public class FakeQuickFixDisplayer : IDisplayQuickFixSelection
		{
			ISelectQuickFix selection = MockRepository.GenerateMock<ISelectQuickFix>();
			
			public Action<ISelectQuickFix> Hidden {get;set;}
			
			public FakeQuickFixDisplayer (IRefactorTask task) {
				selection.Expect(s=>s.Selected).Return(task);
			}
			
			public void InvokeHidden(){
				Hidden(selection);
			}
			
			public void DisplaySelectionDialog (System.Collections.Generic.IEnumerable<MonoDevelop.Stereo.Refactoring.QuickFixes.IRefactorTask> tasks)
			{
				throw new System.NotImplementedException ();
			}
		}
		
		[TearDown]
		public void TearDown(){
			selectedTask.BackToRecord (BackToRecordOptions.All);
			selectedTask.Replay ();
		}
		
		[Test]
		public void Sets_up_hidden_to_run_selected_fix () {
			fixSelection = new FakeQuickFixDisplayer(selectedTask);
			subject = new QuickFixesController(fixSelection);
			
			fixSelection.InvokeHidden();
			
			selectedTask.AssertWasCalled(t=>t.Run (null));
		}
		
		[Test]
		public void Sets_up_hidden_to_run_nothing_when_nothing_was_selected () {
			fixSelection = new FakeQuickFixDisplayer(null);
			subject = new QuickFixesController(fixSelection);
			
			fixSelection.InvokeHidden();
			
			selectedTask.AssertWasNotCalled(t=>t.Run (Arg<RefactoringOptions>.Is.Anything));
		}
	}
	
	[TestFixture]
	public class ProcessSelection
	{
		//TODO: add tests.
	}
}

