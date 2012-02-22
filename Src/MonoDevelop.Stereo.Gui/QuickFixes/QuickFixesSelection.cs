using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using System.Collections.Generic;
using Gtk;
using System.Linq;
using MonoDevelop.Ide;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Gui
{
	public partial class QuickFixesSelection : Gtk.Window, ISelectQuickFix
	{
		public IRefactorTask Selected {
			get;
			private set;
		}
		
		Dictionary<string, IRefactorTask> tasksCache = new Dictionary<string, IRefactorTask>();
		
		public QuickFixesSelection(IEnumerable<MonoDevelop.Stereo.Refactoring.QuickFixes.IRefactorTask> tasks) : base(Gtk.WindowType.Toplevel){
			// Widget MonoDevelop.Stereo.Gui.QuickFixesSelection
			this.Name = "MonoDevelop.Stereo.Gui.QuickFixesSelection";
			this.KeepAbove = true;
			
			this.Opacity = 0.75;
			this.DefaultWidth = 285;
			this.DefaultHeight = 25;
			
			this.Decorated = false;
			this.TypeHint = Gdk.WindowTypeHint.PopupMenu;
			this.WindowPosition = WindowPosition.CenterAlways;
			
			if (Children.Contains(vTaskBox))
				this.Remove(vTaskBox);
			global::Stetic.Gui.Initialize (this);
			
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
				btn.SetAlignment(0.00f, 0.50f);
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
			Selected = null;
		}
	}
}

