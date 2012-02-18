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
//			selectionDisplay = new QuickFixesSelectionWidget();
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
	
	public class Tmp : IRefactorTask
	{
		public bool IsValid ()
		{
			return true;
		}
	
		public void Run (MonoDevelop.Refactoring.RefactoringOptions options)
		{
		}
	
		public string Title {
			get {
				return "Something stupid just for a test";
			}
		}		
	}
	
	public interface IQuickFixesController
	{
		void ProcessSelection(IEnumerable<IRefactorTask> tasks, RefactoringOptions options);
	}
}

