using System;
using MonoDevelop.Refactoring;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Gui
{
	public interface IRefactorWithNaming
	{
		List<Change> PerformChanges (RefactoringOptions options, object properties);
		string OperationTitle {get;}
	}
}

