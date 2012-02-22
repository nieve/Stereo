using System;
using System.Collections.Generic;
using MonoDevelop.Stereo.Gui;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public interface ISelectQuickFix
	{
		IRefactorTask Selected {get;}
	}	
}

