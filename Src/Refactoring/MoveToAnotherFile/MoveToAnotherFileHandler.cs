using System.Collections.Generic;
using System;
using MonoDevelop.Refactoring;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.MoveToAnotherFile
{
	public class MoveToAnotherFileHandler : AbstractRefactoringCommandHandler
	{
		MoveToAnotherFileRefactoring moveToAnotherFileRefactoring = new MoveToAnotherFileRefactoring();
		IProvideRefactoringTasks provider;
		IEnumerable<IRefactorTask> validTasks;
		
		public MoveToAnotherFileHandler ()
		{
			provider = new RefactoringTasksProvider();
		}
		
		public MoveToAnotherFileHandler (IProvideRefactoringTasks provider)
		{
			this.provider = provider;
		}
		
		protected override void Run (RefactoringOptions options)
		{
			if (validTasks.Any()) {
				if (validTasks.Count() == 1) validTasks.First ().Run (options);
				//TODO: else => pass validTasks for user to decide which to use, need Title propg for display purpose
			}
	      	//moveToAnotherFileRefactoring.Run(options);
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			var possibleTasks = provider.GetPossibleRefactoring();
			validTasks = possibleTasks.Where(t=>t.IsValid()).ToList();
			
			info.Enabled = validTasks.Any();
		}
	}
	
	public interface IProvideRefactoringTasks
	{
		List<IRefactorTask> GetPossibleRefactoring();
	}
	
	public interface IRefactorTask //TODO: used on Quick Fix Refactoring (MoveToAnotherFile & GenerateNewType); Add Title propg for display purpose
	{
		bool IsValid();
		void Run (RefactoringOptions options);
	}
	
	public class RefactoringTasksProvider : IProvideRefactoringTasks
	{
		public List<IRefactorTask> GetPossibleRefactoring() {
			throw new NotImplementedException ();
		}
	}
}

