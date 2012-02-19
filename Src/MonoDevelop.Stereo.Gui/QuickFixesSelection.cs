using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using System.Collections.Generic;
using Gtk;

namespace MonoDevelop.Stereo.Gui
{
	public partial class QuickFixesSelection : Gtk.Window, ISelectQuickFix
	{
		public IRefactorTask Selected {
			get;
			private set;
		}
		
		Dictionary<string, IRefactorTask> tasksCache = new Dictionary<string, IRefactorTask>();
		
		public QuickFixesSelection () : 
				base(Gtk.WindowType.Toplevel)
		{
//			this.Build ();
		}

		public void GetSelectedFix (IEnumerable<MonoDevelop.Stereo.Refactoring.QuickFixes.IRefactorTask> tasks)
		{
			this.Remove(vTaskBox);
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Stereo.Gui.QuickFixesSelection
			this.Name = "MonoDevelop.Stereo.Gui.QuickFixesSelection";
			this.Title = global::Mono.Unix.Catalog.GetString ("QuickFixesSelection");
			
			// Container child MonoDevelop.Stereo.Gui.QuickFixesSelection.Gtk.Container+ContainerChild
			this.vTaskBox = new global::Gtk.VBox ();
			this.vTaskBox.Name = "vTaskBox";
			this.vTaskBox.Spacing = 0;
			
			tasksCache.Clear();
			foreach (IRefactorTask task in tasks) {
				tasksCache.Add(task.Title, task);
				var btn = new Button ();
				btn.CanFocus = true;
				btn.UseUnderline = true;
				btn.Label = task.Title;
				this.vTaskBox.Add (btn);
				btn.Clicked += (sender, e) => {
					Selected = tasksCache[((Button)sender).Label];
					Hide (); };
				global::Gtk.Box.BoxChild w = ((global::Gtk.Box.BoxChild)(this.vTaskBox [btn]));
				w.Position = 0;
				w.Expand = false;
				w.Fill = false;
			}
			
			this.Add (this.vTaskBox);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 285;
			this.DefaultHeight = 25;
			
			Decorated = false;
			this.TypeHint = Gdk.WindowTypeHint.PopupMenu;
			Selected = null;
			WindowPosition = WindowPosition.Mouse;
			
			this.Show();
		}
	}
}

