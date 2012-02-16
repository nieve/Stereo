using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class RefactoringTasksProvider : IProvideRefactoringTasks
	{
		public IEnumerable<IRefactorTask> GetPossibleRefactoring() {
			var assembly = System.Reflection.Assembly.GetAssembly(typeof(IRefactorTask));
			
			var foundTasks = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.Name == "IRefactorTask"));
			return foundTasks.Select(t=>assembly.CreateInstance(t.FullName) as IRefactorTask);
		}
	}
	
	public interface IProvideRefactoringTasks
	{
		IEnumerable<IRefactorTask> GetPossibleRefactoring();
	}
}

