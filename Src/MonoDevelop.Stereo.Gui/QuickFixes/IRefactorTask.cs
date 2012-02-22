using System;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public interface IRefactorTask
	{
		bool IsValid();
		void Run (RefactoringOptions options);
		string Title {get;}
		int Position {get;}
	}
}

