using System;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public interface ISelectQuickFix
	{
		void GetSelectedFix(IEnumerable<IRefactorTask> tasks);
		Action<ISelectQuickFix> OnHid {get;set;}
		IRefactorTask Selected {get;}
	}
}

