using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Ide.ProgressMonitoring;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Refactoring;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Refactoring.Rename;
using MonoDevelop.Ide;
using Gtk;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public class RenameNamespaceRefactoring : RenameRefactoring
	{
		public RenameNamespaceRefactoring ()
		{
			this.Name = "Rename Namespace";
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			return options.ResolveResult is NamespaceResolveResult;
		}
		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "_Rename";
	    }
		
		public override void Run (RefactoringOptions options)
		{
			MessageService.ShowCustomDialog((Dialog) new RenameNamespaceItemDialog(options, this));
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object prop)
		{
      		RenameRefactoring.RenameProperties renameProperties = (RenameRefactoring.RenameProperties) prop;
			List<Change> changes = new List<Change>();
			NamespaceReferenceFinder finder = new NamespaceReferenceFinder();
			using (MessageDialogProgressMonitor dialogProgressMonitor = new MessageDialogProgressMonitor(true, false, false, true)) {
		        IEnumerable<MemberReference> references = finder.FindReferences((NamespaceResolveResult)options.ResolveResult, (IProgressMonitor) dialogProgressMonitor);
		        if (references == null)
		          return changes;
				foreach (MemberReference memberReference in references)
		        {
					TextReplaceChange textReplaceChange = new TextReplaceChange();
					textReplaceChange.FileName = (string) memberReference.FileName;
					textReplaceChange.Offset = memberReference.Position;
					textReplaceChange.RemovedChars = memberReference.Name.Length;
					textReplaceChange.InsertedText = renameProperties.NewName;
					textReplaceChange.Description = string.Format(GettextCatalog.GetString("Replace '{0}' with '{1}'"), (object) memberReference.Name, (object) renameProperties.NewName);
					changes.Add((Change) textReplaceChange);
		        }
			}
			return changes;
		}
	}
}

