using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using Rhino.Mocks;
using NUnit.Framework;
using System.Collections.Generic;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.QuickFixesControllerTest
{
	[TestFixture]
	public class ProcessSelection
	{
		FakeQuickFixSelector fixSelection = new FakeQuickFixSelector();
		QuickFixesController subject;
		IRefactorTask selectedTask = MockRepository.GenerateStub<IRefactorTask>();
		
		public class FakeQuickFixSelector : ISelectQuickFix
		{
			public Action<ISelectQuickFix> OnHid {get;set;}
			IRefactorTask selected;
			
			public void InvokeHidden(){
				OnHid(this);
			}
			
			public void GetSelectedFix (System.Collections.Generic.IEnumerable<MonoDevelop.Stereo.Refactoring.QuickFixes.IRefactorTask> tasks)
			{
				throw new System.NotImplementedException ();
			}
			
			public void SetSelected(IRefactorTask task){
				selected = task;
			}
	
			public MonoDevelop.Stereo.Refactoring.QuickFixes.IRefactorTask Selected {
				get {
					return selected;
				}
			}
		}
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new QuickFixesController(fixSelection);
		}
		
		[TearDown]
		public void TearDown(){
			selectedTask.BackToRecord (BackToRecordOptions.All);
			selectedTask.Replay ();
		}
		
		[Test]
		public void Runs_selected_fix () {
			fixSelection.SetSelected(selectedTask);
			
			fixSelection.InvokeHidden();
			
			selectedTask.AssertWasCalled(t=>t.Run (null));
		}
		
		[Test]
		public void Runs_nothing_when_nothing_was_selected () {
			fixSelection.SetSelected(null);
			
			fixSelection.InvokeHidden();
			
			selectedTask.AssertWasNotCalled(t=>t.Run (Arg<RefactoringOptions>.Is.Anything));
		}
	}
}

