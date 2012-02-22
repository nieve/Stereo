using System;
using MonoDevelop.Stereo.Refactoring.QuickFixes;

namespace MonoDevelop.Stereo.Gui
{
	public class CancelRefactoring : IRefactorTask
	{
		public bool IsValid ()
		{
			throw new NotImplementedException();
		}
	
		public void Run (MonoDevelop.Refactoring.RefactoringOptions options)
		{
		}
	
		public string Title {
			get {
				return "Cancel";
			}
		}
		
		public int Position { get {return int.MinValue;}}
	}
}

