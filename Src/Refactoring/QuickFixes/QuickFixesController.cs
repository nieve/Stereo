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
			selectionDisplay.GetSelectedFix(tasks);			
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

