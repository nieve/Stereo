using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Gui
{
	public interface IDisplayQuickFixSelection
	{
		void DisplaySelectionDialog(IEnumerable<IRefactorTask> tasks);
		Action<ISelectQuickFix> SelectionMade {get;set;}
	}
	
	public class QuickFixSelectionDisplayer : IDisplayQuickFixSelection
	{
		public Action<ISelectQuickFix> SelectionMade {get;set;}
		
		public void DisplaySelectionDialog (IEnumerable<MonoDevelop.Stereo.Refactoring.QuickFixes.IRefactorTask> tasks)
		{
			var dialog = new QuickFixesSelection(tasks);
			
			dialog.Hidden += delegate(object sender, EventArgs e) {
				SelectionMade(dialog);
			};
			dialog.Show();
		}
	}
}

