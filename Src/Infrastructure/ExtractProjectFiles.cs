using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;

namespace MonoDevelop.Stereo {
	public interface IExtractProjectFiles{
		IEnumerable<Tuple<ProjectDom, FilePath>> GetFileNames(Solution solution, ProjectDom dom, ICompilationUnit unit, IProgressMonitor monitor);
	}
	
	public class ExtractProjectFiles : IExtractProjectFiles {
		public IEnumerable<Tuple<ProjectDom, FilePath>> GetFileNames(Solution solution, ProjectDom dom, ICompilationUnit unit, IProgressMonitor monitor)
	    {
			int counter = 0;
			ReadOnlyCollection<Project> allProjects = solution.GetAllProjects();
			if (monitor != null)
				monitor.BeginTask(GettextCatalog.GetString("Finding references in solution..."), 
          			allProjects.Sum<Project>(p => p.Files.Count));
				foreach (Project project in allProjects) {
					if (monitor != null && monitor.IsCancelRequested) yield break;
					ProjectDom currentDom = ProjectDomService.GetProjectDom(project);
					foreach (ProjectFile projectFile in (Collection<ProjectFile>) project.Files) {
						if (monitor != null && monitor.IsCancelRequested) yield break;
						yield return Tuple.Create<ProjectDom, FilePath>(currentDom, projectFile.FilePath);
						if (monitor != null) {
							if (counter % 10 == 0) monitor.Step(10);
							++counter;
						}
					}
				}
	          if (monitor != null) monitor.EndTask();
	    }
	}
}