using System;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public interface ISelectQuickFix
	{
		IRefactorTask GetSelectedFix(IEnumerable<IRefactorTask> tasks);
	}
}

