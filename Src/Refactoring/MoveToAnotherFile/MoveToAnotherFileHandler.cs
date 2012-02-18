using System.Collections.Generic;
using System;
using MonoDevelop.Refactoring;
using System.Linq;
using MonoDevelop.Stereo.Refactoring.QuickFixes;

namespace MonoDevelop.Stereo.Refactoring.MoveToAnotherFile
{
	public class MoveToAnotherFileHandler : AbstractRefactoringCommandHandler
	{
		MoveToAnotherFileRefactoring moveToAnotherFileRefactoring = new MoveToAnotherFileRefactoring();
		IProvideRefactoringTasks provider;
		IQuickFixesController controller;
		IEnumerable<IRefactorTask> validTasks;
		
		public MoveToAnotherFileHandler ()
		{
			provider = new RefactoringTasksProvider();
			controller = new QuickFixesController();
		}
		
		public MoveToAnotherFileHandler (IProvideRefactoringTasks provider, IQuickFixesController controller)
		{
			this.provider = provider;
			this.controller = controller;
		}
		
		protected override void Run (RefactoringOptions options)
		{
			if (validTasks.Any()) {
				if (validTasks.Count() == 1) 
					validTasks.First ().Run (options);
				else if (validTasks.Count() > 1)
					controller.DisplayPossibilities(validTasks);
			}
	      	//moveToAnotherFileRefactoring.Run(options);
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			var possibleTasks = provider.GetPossibleRefactoring();
			validTasks = possibleTasks.Where(t=>t.IsValid());
			
			info.Enabled = validTasks.Any();
		}
	}
	
	
}