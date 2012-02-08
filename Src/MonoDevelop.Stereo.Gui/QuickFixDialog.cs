using System;
using MonoDevelop.Refactoring;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Ide;

namespace MonoDevelop.Stereo.Gui
{
	public partial class QuickFixDialog : Gtk.Dialog
	{
		RefactoringOperation refactoring;
		RefactoringOptions options;
		
		public QuickFixDialog (RefactoringOptions options, RefactoringOperation refactoring)
		{
			this.refactoring = refactoring;
			this.options = options;
			this.Build ();
			buttonOk.Clicked += OnOKClicked;
			this.Title = "Quick Fix";
			this.label1.Text = "Are you sure you want to move this type to a new file?";
		}
		
		void OnOKClicked (object sender, EventArgs e)
		{
			this.Destroy ();
			List<Change> changes = refactoring.PerformChanges (options, null);
			IProgressMonitor monitor = IdeApp.Workbench.ProgressMonitors.GetBackgroundProgressMonitor (this.Title, null);
			RefactoringService.AcceptChanges (monitor, options.Dom, changes);
		}
	}
}

