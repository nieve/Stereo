using System;
using System.Collections.Generic;
using NUnit.Framework;
using MonoDevelop.Stereo.Refactoring.MoveToAnotherFile;
using Rhino.Mocks;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.MoveToAnotherFileHandlerTest
{
	public class TestedMoveToAnotherFileHandler : MoveToAnotherFileHandler
	{
		public TestedMoveToAnotherFileHandler (IProvideRefactoringTasks provider) : base(provider) { }
		public void Update (){base.Update (new MonoDevelop.Components.Commands.CommandInfo());}
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
			
			subject = new TestedMoveToAnotherFileHandler(provider);
		}
		
		[Test]
		public void Marks_valid_task_for_use_when_running() {
			subject.Update();
			subject.Run();
			
			validTask.AssertWasCalled(t=>t.Run(Arg<RefactoringOptions>.Is.Anything));
		}
	}
}

