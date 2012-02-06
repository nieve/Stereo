using System;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring
{
	public class MoveToAnotherFileHandler : AbstractRefactoringCommandHandler
	{
		MoveToAnotherFileRefactoring moveToAnotherFileRefactoring = new MoveToAnotherFileRefactoring();
		public MoveToAnotherFileHandler ()
		{
		}
		
		protected override void Run (RefactoringOptions options)
		{
	      	moveToAnotherFileRefactoring.Run(options);
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			info.Enabled = moveToAnotherFileRefactoring.IsValid(null);
		}
	}
}

