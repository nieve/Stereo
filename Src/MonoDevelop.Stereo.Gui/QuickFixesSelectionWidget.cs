using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;

namespace MonoDevelop.Stereo.Gui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class QuickFixesSelectionWidget : Gtk.Bin, ISelectQuickFix
	{
		public QuickFixesSelectionWidget ()
		{
			this.Build ();
		}

		public IRefactorTask GetSelectedFix (System.Collections.Generic.IEnumerable<IRefactorTask> tasks)
		{
			throw new System.NotImplementedException ();
		}
	}
}

