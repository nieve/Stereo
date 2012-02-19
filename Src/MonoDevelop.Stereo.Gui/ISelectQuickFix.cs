using System;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public interface ISelectQuickFix
	{
		void GetSelectedFix(IEnumerable<IRefactorTask> tasks);
		event EventHandler Hidden;
		IRefactorTask Selected {get;}
	}
}

