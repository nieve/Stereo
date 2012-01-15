using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Projects.Dom;

namespace MonoDevelop.Stereo.Refactoring.GenerateClass
{
	public class GenerateClassHandler : AbstractRefactoringCommandHandler
	{
		GenerateClassRefactoring methodRefactoring = new GenerateClassRefactoring();
		protected override void Run (RefactoringOptions options)
		{
	      	methodRefactoring.Run(options);
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			info.Enabled = methodRefactoring.IsValid(null);
		}
		
		public GenerateClassHandler ()
		{
		}
	}
}

