
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Stereo.Gui
{
	public partial class SomeWidget
	{
		private global::Gtk.VButtonBox vbuttonbox1;
		private global::Gtk.Button btnMoveToNewFile;
		private global::Gtk.Button btnElse;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Stereo.Gui.SomeWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.Stereo.Gui.SomeWidget";
			// Container child MonoDevelop.Stereo.Gui.SomeWidget.Gtk.Container+ContainerChild
			this.vbuttonbox1 = new global::Gtk.VButtonBox ();
			this.vbuttonbox1.Name = "vbuttonbox1";
			this.vbuttonbox1.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(1));
			// Container child vbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
			this.btnMoveToNewFile = new global::Gtk.Button ();
			this.btnMoveToNewFile.WidthRequest = 200;
			this.btnMoveToNewFile.CanFocus = true;
			this.btnMoveToNewFile.Name = "btnMoveToNewFile";
			this.btnMoveToNewFile.UseUnderline = true;
			this.btnMoveToNewFile.Xalign = 0F;
			this.btnMoveToNewFile.Label = global::Mono.Unix.Catalog.GetString ("Move To A New File");
			this.vbuttonbox1.Add (this.btnMoveToNewFile);
			global::Gtk.ButtonBox.ButtonBoxChild w1 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox1 [this.btnMoveToNewFile]));
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
			this.btnElse = new global::Gtk.Button ();
			this.btnElse.CanFocus = true;
			this.btnElse.Name = "btnElse";
			this.btnElse.UseUnderline = true;
			this.btnElse.Xalign = 0F;
			this.btnElse.Label = global::Mono.Unix.Catalog.GetString ("Do Something Else");
			this.vbuttonbox1.Add (this.btnElse);
			global::Gtk.ButtonBox.ButtonBoxChild w2 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox1 [this.btnElse]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.Add (this.vbuttonbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}
