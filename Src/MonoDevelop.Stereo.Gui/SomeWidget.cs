using System;

namespace MonoDevelop.Stereo.Gui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SomeWidget : Gtk.Bin
	{
		public SomeWidget ()
		{
			this.Build ();
			this.GdkWindow.Opacity = 0.60;
		}
	}
}

