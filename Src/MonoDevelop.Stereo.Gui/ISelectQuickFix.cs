using System;
using System.Collections.Generic;
using MonoDevelop.Stereo.Gui;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public interface ISelectQuickFix
	{
		IRefactorTask Selected {get;}
	}
	
	//TODO: Move these 2 to another file.
	public interface IDisplayQuickFixSelection
	{
		void DisplaySelectionDialog(IEnumerable<IRefactorTask> tasks);
		Action<ISelectQuickFix> Hidden {get;set;}
	}
	
	public class QuickFixSelectionDisplayer : IDisplayQuickFixSelection
	{
		public Action<ISelectQuickFix> Hidden {get;set;}
		
		public void DisplaySelectionDialog (IEnumerable<MonoDevelop.Stereo.Refactoring.QuickFixes.IRefactorTask> tasks)
		{
			var dialog = new QuickFixesSelection(tasks);
			
			dialog.Hidden += delegate(object sender, EventArgs e) {
				Hidden(dialog);
			};
		}
	}
}

