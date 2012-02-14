using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Projects.Dom;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public class GenerateNewTypeHandler : AbstractRefactoringCommandHandler
	{
		GenerateNewTypeRefactoring newTypeGenRefactoring = new GenerateNewTypeRefactoring();
		protected override void Run (RefactoringOptions options)
		{
	      	newTypeGenRefactoring.Run(options);
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			info.Enabled = newTypeGenRefactoring.IsValid();
		}
		
		public GenerateNewTypeHandler ()
		{
		}
	}
}

