using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.QuickFixes
{
	public class RefactoringTasksProvider : IProvideRefactoringTasks
	{
		public IEnumerable<IRefactorTask> GetPossibleRefactoring() {
			var assembly = System.Reflection.Assembly.GetAssembly(typeof(IProvideRefactoringTasks));
			
			var foundTasks = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.Name == "IRefactorTask") && !t.IsAbstract && !t.IsInterface);
			return foundTasks.Select(t=>assembly.CreateInstance(t.FullName) as IRefactorTask);
		}
	}
	
	public interface IProvideRefactoringTasks
	{
		IEnumerable<IRefactorTask> GetPossibleRefactoring();
	}
}

