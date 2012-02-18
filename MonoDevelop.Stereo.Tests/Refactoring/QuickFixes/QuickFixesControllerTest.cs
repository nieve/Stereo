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
		ISelectQuickFix fixSelection = MockRepository.GenerateStub<ISelectQuickFix>();
		QuickFixesController subject;
		IRefactorTask selectedTask = MockRepository.GenerateStub<IRefactorTask>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new QuickFixesController(fixSelection);
		}
		
		[TearDown]
		public void TearDown(){
			fixSelection.BackToRecord (BackToRecordOptions.All);
			fixSelection.Replay ();
			
			selectedTask.BackToRecord (BackToRecordOptions.All);
			selectedTask.Replay ();
		}
		
		[Test]
		public void Runs_selected_fix ()
		{
			fixSelection.Stub (fs=>fs.GetSelectedFix(Arg<IEnumerable<IRefactorTask>>.Is.Anything)).Return (selectedTask);
			
			subject.ProcessSelection (null, null);
			
			selectedTask.AssertWasCalled(t=>t.Run (null));
		}
		
		[Test]
		public void Runs_nothing_when_nothing_was_selected ()
		{
			fixSelection.Stub (fs=>fs.GetSelectedFix(Arg<IEnumerable<IRefactorTask>>.Is.Anything)).Return (null);
			
			subject.ProcessSelection (null, null);
			
			selectedTask.AssertWasNotCalled(t=>t.Run (Arg<RefactoringOptions>.Is.Anything));
		}
	}
}

