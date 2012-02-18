using System.Collections.Generic;
using System.Linq;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Refactoring.MoveToAnotherFile;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using NUnit.Framework;
using Rhino.Mocks;

namespace MonoDevelop.Stereo.MoveToAnotherFileHandlerTest
{
	public class TestedMoveToAnotherFileHandler : MoveToAnotherFileHandler
	{
		public TestedMoveToAnotherFileHandler (IProvideRefactoringTasks provider, IQuickFixesController controller) : base(provider, controller) { }
		public void Update (){base.Update (new MonoDevelop.Components.Commands.CommandInfo());}
		public new void Update (MonoDevelop.Components.Commands.CommandInfo info){base.Update (info);}
		public new void Run(){base.Run((RefactoringOptions)null);}
	}
	
	[TestFixture]
	public class Update
	{
		TestedMoveToAnotherFileHandler subject;
		IProvideRefactoringTasks provider = MockRepository.GenerateStub<IProvideRefactoringTasks>();
		IRefactorTask validTask = MockRepository.GenerateStub<IRefactorTask>();
		IRefactorTask invalidTask = MockRepository.GenerateStub<IRefactorTask>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			validTask.Stub(t=>t.IsValid()).Return(true);
			invalidTask.Stub(t=>t.IsValid()).Return(false);
			provider.Stub(p=>p.GetPossibleRefactoring()).Return(new List<IRefactorTask>{invalidTask, validTask});
			
			subject = new TestedMoveToAnotherFileHandler(provider, null);
		}
		
		[Test()]
		public void Enables_command_info_when_any_valid_tasks_were_found ()
		{
			var info = new MonoDevelop.Components.Commands.CommandInfo{Enabled = false};
			subject.Update(info);
			
			Assert.IsTrue(info.Enabled, "Command info was not enabled as expected");
		}
	}
	
	
	
	[TestFixture]
	public class Run
	{
		TestedMoveToAnotherFileHandler subject;
		IProvideRefactoringTasks provider = MockRepository.GenerateStub<IProvideRefactoringTasks>();
		IRefactorTask validTask = MockRepository.GenerateStub<IRefactorTask>();
		IRefactorTask anotherValidTask = MockRepository.GenerateStub<IRefactorTask>();
		IRefactorTask someValidTask = MockRepository.GenerateStub<IRefactorTask>();
		IRefactorTask invalidTask = MockRepository.GenerateStub<IRefactorTask>();
		IQuickFixesController controller = MockRepository.GenerateStub<IQuickFixesController>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			validTask.Stub(t=>t.IsValid()).Return(true);
			anotherValidTask.Stub(t=>t.IsValid()).Return(true);
			someValidTask.Stub(t=>t.IsValid()).Return(true);
			invalidTask.Stub(t=>t.IsValid()).Return(false);
			
			subject = new TestedMoveToAnotherFileHandler(provider, controller);
		}
		
		[TearDown]
		public void TearDown(){
			provider.BackToRecord(BackToRecordOptions.All);
			provider.Replay();
		}
		
		[Test]
		public void Runs_the_only_valid_task() {
			provider.Stub(p=>p.GetPossibleRefactoring()).Return(new List<IRefactorTask>{invalidTask, validTask}).Repeat.Once();
			
			subject.Update();
			subject.Run();
			
			validTask.AssertWasCalled(t=>t.Run(Arg<RefactoringOptions>.Is.Anything));
		}
		
		[Test]
		public void Doesnt_run_any_valid_task_when_more_than_one_valid_task_found() {
			provider.Stub(p=>p.GetPossibleRefactoring()).Return(new List<IRefactorTask>{validTask, anotherValidTask, invalidTask}).Repeat.Once();
			
			subject.Update();
			subject.Run();
			
			validTask.AssertWasCalled(t=>t.IsValid());
			validTask.AssertWasNotCalled(t=>t.Run(Arg<RefactoringOptions>.Is.Anything));
			anotherValidTask.AssertWasNotCalled(t=>t.Run(Arg<RefactoringOptions>.Is.Anything));
		}
		
		[Test]
		public void Runs_quick_fixes_controller_when_more_than_one_valid_task_found() {
			List<IRefactorTask> tasks = new List<IRefactorTask>{someValidTask,anotherValidTask,invalidTask};
			provider.Stub(p=>p.GetPossibleRefactoring()).Return(tasks).Repeat.Once();
			
			subject.Update();
			subject.Run();
			
			var tasksArg = Arg<IEnumerable<IRefactorTask>>.Matches(ts=>
				ts.Contains(someValidTask) && ts.Contains(anotherValidTask)
			);
			controller.AssertWasCalled(c=>c.ProcessSelection(tasksArg, Arg<RefactoringOptions>.Is.Anything));
		}
	}
}

