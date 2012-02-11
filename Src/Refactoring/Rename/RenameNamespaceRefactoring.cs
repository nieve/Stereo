using System.Collections.Generic;
using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.ProgressMonitoring;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Refactoring;
using MonoDevelop.Refactoring.Rename;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public class RenameNamespaceRefactoring : RenameRefactoring
	{
		IFindNamespaceReference finder;
		public RenameNamespaceRefactoring ()
		{
			this.Name = "Rename Namespace";
			finder = new NamespaceReferenceFinder();
		}
		
		public RenameNamespaceRefactoring (IFindNamespaceReference namespaceRefFinder)
		{
			this.Name = "Rename Namespace";
			finder = namespaceRefFinder;
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
					textReplaceChange.InsertedText = renameProperties.NewName;
					textReplaceChange.Description = string.Format(GettextCatalog.GetString("Replace '{0}' with '{1}'"), (object) memberReference.Name, (object) renameProperties.NewName);
					changes.Add((Change) textReplaceChange);
		        }
			}
			return changes;
		}
	}
}

