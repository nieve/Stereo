// 
// RenameItemDialog.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using Gtk;

using MonoDevelop.Core;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.CodeGeneration;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Ide.ProgressMonitoring;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public partial class RenameNamespaceItemDialog : Gtk.Dialog
	{
		RenameNamespaceRefactoring rename;
		RefactoringOptions options;
		
		public RenameNamespaceItemDialog (RefactoringOptions options, RenameNamespaceRefactoring rename)
		{
			this.options = options;
			this.rename = rename;
			this.Build ();
			
			this.Title = GettextCatalog.GetString ("Rename Namespace");
			
			entry.SelectRegion (0, -1);
			
			buttonPreview.Sensitive = buttonOk.Sensitive = false;
			entry.Changed += OnEntryChanged;
			entry.Activated += OnEntryActivated;
			
			buttonOk.Clicked += OnOKClicked;
			buttonPreview.Clicked += OnPreviewClicked;
			entry.Changed += delegate { buttonPreview.Sensitive = buttonOk.Sensitive = ValidateName (); };
			ValidateName ();
		}

		bool ValidateName ()
		{
			INameValidator nameValidator = new NamespaceValidator();
			ValidationResult result = nameValidator.ValidateName (this.options.SelectedItem, entry.Text);
			if (!result.IsValid) {
				imageWarning.IconName = Gtk.Stock.DialogError;
			} else if (result.HasWarning) {
				imageWarning.IconName = Gtk.Stock.DialogWarning;
			} else {
				imageWarning.IconName = Gtk.Stock.Apply;
			}
			labelWarning.Text = result.Message;
			return result.IsValid;
		}

		void OnEntryChanged (object sender, EventArgs e)
		{
			// Don't allow the user to click OK unless there is a new name
			buttonPreview.Sensitive = buttonOk.Sensitive = entry.Text.Length > 0;
		}

		void OnEntryActivated (object sender, EventArgs e)
		{
			if (buttonOk.Sensitive)
				buttonOk.Click ();
		}
		
		RenameNamespaceRefactoring.RenameProperties Properties {
			get {
				return new RenameNamespaceRefactoring.RenameProperties () {
					NewName = entry.Text,
					RenameFile = renameFileFlag.Visible && renameFileFlag.Active
				};
			}
		}
		
		void OnOKClicked (object sender, EventArgs e)
		{
			var properties = Properties;
			((Widget)this).Destroy ();
			List<Change> changes = rename.PerformChanges (options, properties);
			IProgressMonitor monitor = IdeApp.Workbench.ProgressMonitors.GetBackgroundProgressMonitor (this.Title, null);
			RefactoringService.AcceptChanges (monitor, options.Dom, changes);
		}
		
		void OnPreviewClicked (object sender, EventArgs e)
		{
			var properties = Properties;
			((Widget)this).Destroy ();
			List<Change> changes = rename.PerformChanges (options, properties);
			MessageService.ShowCustomDialog (new RefactoringPreviewDialog (options.Dom, changes));
		}
	}
		
}
