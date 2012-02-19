using System.Collections.Generic;
using System;
using MonoDevelop.Refactoring;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class QuickFixesHandler : AbstractRefactoringCommandHandler
	{
		IProvideRefactoringTasks provider;
		IQuickFixesController controller;
		IEnumerable<IRefactorTask> validTasks;
		
		public QuickFixesHandler ()
		{
			provider = new RefactoringTasksProvider();
			controller = new QuickFixesController();
		}
		
		public QuickFixesHandler (IProvideRefactoringTasks provider, IQuickFixesController controller)
		{
			this.provider = provider;
			this.controller = controller;
		}
		
		protected override void Run (RefactoringOptions options)
		{
			if (validTasks.Any())
					controller.ProcessSelection(validTasks, options);
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			var possibleTasks = provider.GetPossibleRefactoring();
			validTasks = possibleTasks.Where(t=>t.IsValid());
			
			info.Enabled = validTasks.Any();
		}
	}
}