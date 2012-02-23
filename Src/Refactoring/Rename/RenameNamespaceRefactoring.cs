using System.Collections.Generic;
using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.ProgressMonitoring;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Refactoring;
using MonoDevelop.Refactoring.Rename;
using MonoDevelop.Stereo.Gui;
using System;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public class RenameNamespaceRefactoring : RenameRefactoring, IRefactorWithNaming
	{
		IFindNamespaceReference finder;
		INameValidator validator;
		
		public RenameNamespaceRefactoring () : this(new NamespaceReferenceFinder(), new NamespaceValidator()) {	}
		
		public RenameNamespaceRefactoring (IFindNamespaceReference namespaceRefFinder, INameValidator validator)
		{
			this.Name = "Rename Namespace";
			finder = namespaceRefFinder;
			this.validator = validator;
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			return options.ResolveResult is NamespaceResolveResult;
		}
		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "_Rename";
	    }
		
		public string OperationTitle  {
			get{return "Rename Namespace";}
		}
		
		public override void Run (RefactoringOptions options)
		{
			MessageService.ShowCustomDialog((Dialog) new RefactoringNamingDialog(options, this, new NamespaceValidator()));
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object prop)
		{
      		string newName = (string) prop;
			List<Change> changes = new List<Change>();
			using (var dialogProgressMonitor = new MessageDialogProgressMonitor(true, false, false, true)) {
		        var references = finder.FindReferences((NamespaceResolveResult)options.ResolveResult, dialogProgressMonitor);
		        if (references == null)
		          return changes;
				foreach (MemberReference memberReference in references)
		        {
					TextReplaceChange textReplaceChange = new TextReplaceChange();
					textReplaceChange.FileName = (string) memberReference.FileName;
					textReplaceChange.Offset = memberReference.Position;
					textReplaceChange.RemovedChars = memberReference.Name.Length;
					textReplaceChange.InsertedText = newName;
					textReplaceChange.Description = string.Format(GettextCatalog.GetString("Replace '{0}' with '{1}'"), (object) memberReference.Name, (object) newName);
					changes.Add((Change) textReplaceChange);
		        }
			}
			return changes;
		}
	}
}

