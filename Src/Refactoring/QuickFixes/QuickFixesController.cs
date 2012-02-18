using System;
using System.Collections.Generic;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class QuickFixesController : IQuickFixesController
	{
		ISelectQuickFix selectionDisplay;
		
		public QuickFixesController ()
		{
			selectionDisplay = new QuickFixesSelectionWidget();
		}
		
		public QuickFixesController (ISelectQuickFix selectionDisplay)
		{
			this.selectionDisplay = selectionDisplay;
		}
		
		public void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options){
			IRefactorTask selectedFix = selectionDisplay.GetSelectedFix(tasks);
			if (selectedFix != null) selectedFix.Run(options);
		}
	}
	
	public interface IQuickFixesController
	{
		void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options);
	}
}

