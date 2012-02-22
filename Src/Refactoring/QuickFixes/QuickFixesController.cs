using System;
using System.Collections.Generic;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class QuickFixesController : IQuickFixesController
	{
		IDisplayQuickFixSelection selectionDisplayer;
		
		public QuickFixesController () : this(new QuickFixSelectionDisplayer())
		{
		}
		
		public QuickFixesController (IDisplayQuickFixSelection selectionDisplayer)
		{
			this.selectionDisplayer = selectionDisplayer;
			this.selectionDisplayer.SelectionMade = (dialog) => {
				IRefactorTask selectedFix = dialog.Selected;
				if (selectedFix != null)
					selectedFix.Run (Options);
			};
		}
		
		RefactoringOptions Options{set; get;}
		
		public void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options){
			Options = options;
			var displayableTasks = tasks.ToList();
			displayableTasks.Add(new CancelRefactoring());
			var orderedTasks = displayableTasks.OrderBy(t=>t.Position);
			selectionDisplayer.DisplaySelectionDialog(orderedTasks);
		}
	}
	
	public interface IQuickFixesController
	{
		void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options);
	}
}

