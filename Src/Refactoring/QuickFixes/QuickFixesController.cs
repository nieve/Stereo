using System;
using System.Collections.Generic;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class QuickFixesController : IQuickFixesController
	{
		IDisplayQuickFixSelection selectionDisplay;
		
		public QuickFixesController () : this(new QuickFixSelectionDisplayer())
		{
		}
		
		public QuickFixesController (IDisplayQuickFixSelection selectionDisplay)
		{
			this.selectionDisplay = selectionDisplay;
			selectionDisplay.Hidden = (dialog) => {
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
			selectionDisplay.DisplaySelectionDialog(orderedTasks);
		}
	}
	
	public interface IQuickFixesController
	{
		void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options);
	}
}

