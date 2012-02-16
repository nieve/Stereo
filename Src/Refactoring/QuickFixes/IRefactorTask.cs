using System;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public interface IRefactorTask //TODO: used on Quick Fix Refactoring (MoveToAnotherFile & GenerateNewType); Add Title propg for display purpose
	{
		bool IsValid();
		void Run (RefactoringOptions options);
	}
}

