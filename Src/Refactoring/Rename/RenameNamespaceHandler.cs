using System;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public class RenameNamespaceHandler : AbstractRefactoringCommandHandler
	{
		RenameNamespaceRefactoring renameRefactoring = new RenameNamespaceRefactoring ();
		public RenameNamespaceHandler ()
		{
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			info.Enabled = true;
		}
		
		protected override void Run (RefactoringOptions options)
		{
			if (renameRefactoring.IsValid (options))
			{
				renameRefactoring.Run (options);
			}
		}
	}
}

