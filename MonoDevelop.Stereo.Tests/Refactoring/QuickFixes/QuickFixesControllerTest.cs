using System;
using System.Linq;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using Rhino.Mocks;
using NUnit.Framework;
using System.Collections.Generic;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;
using System.Linq.Expressions;

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
			
			public Action<ISelectQuickFix> SelectionMade {get;set;}
			
			public FakeQuickFixDisplayer (IRefactorTask task) {
				selection.Expect(s=>s.Selected).Return(task);
			}
			
			public void InvokeHidden(){
				SelectionMade(selection);
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
		QuickFixesController subject;
		IDisplayQuickFixSelection selectionDisplayer = MockRepository.GenerateStub<IDisplayQuickFixSelection>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new QuickFixesController(selectionDisplayer);
		}
		
		[Test]
		public void Adds_cancael_option_before_displaying ()
		{
			var someTask = MockRepository.GenerateStub<IRefactorTask>();
			var tasks = new List<IRefactorTask>{someTask};
			
			subject.ProcessSelection (tasks, null);
			
			Expression<Predicate<IEnumerable<IRefactorTask>>> cancelOptionAdded = 
				ts=>ts.Count() == 2 && ts.Contains(someTask) && ts.Any(t=>t is CancelRefactoring);
			selectionDisplayer.AssertWasCalled(d=>d.DisplaySelectionDialog(Arg<IEnumerable<IRefactorTask>>.Matches(cancelOptionAdded)));
		}
		
		[Test]
		public void Orders_tasks_before_displaying ()
		{
			var someTask = MockRepository.GenerateMock<IRefactorTask>();
			someTask.Expect(t=>t.Position).Return(int.MinValue + 1);
			var tasks = new List<IRefactorTask>{someTask};
			
			subject.ProcessSelection (tasks, null);
			
			Expression<Predicate<IEnumerable<IRefactorTask>>> cancelOptionAdded = 
				ts=>ts.First () is CancelRefactoring && ts.Last() == someTask;
			selectionDisplayer.AssertWasCalled(d=>d.DisplaySelectionDialog(Arg<IEnumerable<IRefactorTask>>.Matches(cancelOptionAdded)));
		}
	}
}

