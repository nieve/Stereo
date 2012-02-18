using System;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class QuickFixesController : IQuickFixesController
	{
		public void DisplayPossibilities(IEnumerable<IRefactorTask> tasks){
			throw new NotImplementedException();
			//TODO: show dialog, when choice selected run corresponding task.
		}
	}
	
	public interface IQuickFixesController
	{
		void DisplayPossibilities(IEnumerable<IRefactorTask> tasks);
	}
}

