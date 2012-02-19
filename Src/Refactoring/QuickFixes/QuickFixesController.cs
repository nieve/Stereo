using System;
using System.Collections.Generic;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class QuickFixesController : IQuickFixesController
	{
		ISelectQuickFix selectionDisplay;
		
		public QuickFixesController ()
		{
			selectionDisplay = new QuickFixesSelection();
			SetUpSelectionProcessing();
		}
		
		public QuickFixesController (ISelectQuickFix selectionDisplay)
		{
			this.selectionDisplay = selectionDisplay;
			SetUpSelectionProcessing();
		}
		
		private void SetUpSelectionProcessing(){
			selectionDisplay.Hidden += delegate {
				IRefactorTask selectedFix = selectionDisplay.Selected;
				if (selectedFix != null)
					selectedFix.Run (Options);
			};
		}
		
		RefactoringOptions Options{set; get;}
		
		public void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options){
			Options = options;
			var displayableTasks = tasks.ToList();
			displayableTasks.Add(new CancelRefactoring());
			displayableTasks.Reverse ();
			selectionDisplay.GetSelectedFix(displayableTasks);
		}
	}
	
	public interface IQuickFixesController
	{
		void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options);
	}
}

