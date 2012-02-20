using System;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring.CreateDerivedType
{
	public class CreateDerivedTypeHandler : AbstractRefactoringCommandHandler
	{
		ICreateDerivedTypeRefactoring refactoring;
		
		public CreateDerivedTypeHandler ()
		{
			refactoring = new CreateDerivedTypeRefactoring();
		}
		
		protected override void Run (RefactoringOptions options)
		{
			refactoring.Run(options);
		}
		
		protected override void Update (MonoDevelop.Components.Commands.CommandInfo info)
		{
			info.Enabled = refactoring.IsValid();
		}
	}
}

