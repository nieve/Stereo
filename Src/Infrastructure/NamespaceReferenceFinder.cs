using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.TextEditor;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;

namespace MonoDevelop.Stereo
{
	public interface IFindNamespaceReference
	{
		IEnumerable<MemberReference> FindReferences(NamespaceResolveResult resolveResult, IProgressMonitor monitor);
		IEnumerable<MemberReference> FindReferences(Solution solution, NamespaceResolveResult resolveResult, IProgressMonitor monitor);
	}
	public class NamespaceReferenceFinder : IFindNamespaceReference
	{
		IExtractProjectFiles projectFilesExtractor;
		public NamespaceReferenceFinder ()
		{
			projectFilesExtractor = new ExtractProjectFiles();
		}
		
		public NamespaceReferenceFinder (IExtractProjectFiles extractProjectFiles)
		{
			projectFilesExtractor = extractProjectFiles;
		}
		
		public IEnumerable<MemberReference> FindReferences(NamespaceResolveResult resolveResult, IProgressMonitor monitor){
			return FindReferences(IdeApp.ProjectOperations.CurrentSelectedSolution, resolveResult, monitor);
		}
		
		public IEnumerable<MemberReference> FindReferences(Solution solution, NamespaceResolveResult resolveResult, IProgressMonitor monitor){
	        MonoDevelop.Ide.Gui.Document doc = IdeApp.Workbench.ActiveDocument;
	        ProjectDom dom = doc.Dom;
      		ICompilationUnit unit = doc.CompilationUnit;
			string nspace = resolveResult.Namespace;
      		IEnumerable<INode> searchNodes = (IEnumerable<INode>) new INode[1]{new Namespace(nspace)};
			
			string currentMime = null;
			IEnumerable<Tuple<ProjectDom, FilePath>> projFiles = projectFilesExtractor.GetFileNames (solution, dom, unit, monitor);
			foreach (Tuple<ProjectDom, FilePath> tuple in projFiles) {
				FilePath filePath = tuple.Item2;
				if (string.IsNullOrWhiteSpace(filePath.Extension)) continue;
				if (monitor != null && monitor.IsCancelRequested) {
					break;
				}
				else {
					string mime = DesktopService.GetMimeTypeForUri(filePath);
					currentMime = mime;
  					TextEditorData editor = TextFileProvider.Instance.GetTextEditorData(filePath);
    				IParser parser = ProjectDomService.GetParser(filePath);
    				ParsedDocument parsedDoc = parser.Parse(dom, filePath, editor.Text);
					int lastFoundIndex = 0;
					int column;
					int lineOffset;
					for (var i = 0; i < editor.Lines.Count(); i++){
						var line = editor.GetLineText(i);
						if (line != null && line.Contains("using " + nspace + ";")) {
							column = line.IndexOf (nspace);
							lineOffset = editor.Text.IndexOf(line, lastFoundIndex);
							lastFoundIndex = lineOffset + line.Length;
							yield return new MemberReference(null, filePath, lineOffset + column, i, column, nspace, nspace);
						}
						if (line != null && line.Trim().EndsWith("namespace " + nspace)) {
							column = line.IndexOf (nspace);
							lineOffset = editor.Text.IndexOf(line, lastFoundIndex);
							lastFoundIndex = lineOffset + line.Length;
							yield return new MemberReference(null, filePath, lineOffset + column, i, column, nspace, nspace);
						}
					}
				}
			}
			yield break;
		}
	}
}

