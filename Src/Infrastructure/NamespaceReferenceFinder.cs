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
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Refactoring;

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
						if (line != null && 
						    (line.Trim().StartsWith("namespace " + nspace + ";") || line.Contains("namespace " + nspace + ";"))) {
							column = line.IndexOf (nspace);
							lineOffset = editor.Text.IndexOf(line, lastFoundIndex);
							lastFoundIndex = lineOffset + line.Length;
							yield return new MemberReference(null, filePath, lineOffset + column, i, column, nspace, nspace);
						}
					}
					
					foreach(var memberRef in FindInnerReferences (monitor, nspace, filePath))
						yield return memberRef;
				}
			}
			yield break;
		}
		
		IEnumerable<MemberReference> FindInnerReferences (IProgressMonitor monitor, string nspace, FilePath filePath)
		{
			var document = IdeApp.Workbench.GetDocument(filePath.CanonicalPath);
			if (document != null){
				ITextBuffer editor = document.GetContent<ITextBuffer> ();
				if (editor.Text.Contains(nspace + ".")){
					var position = -1;
					while ((position = editor.Text.IndexOf(nspace + ".", position + 1)) > -1) {
						if (monitor != null && monitor.IsCancelRequested) {
							break;
						}
						int line, column;
						editor.GetLineColumnFromPosition (position + nspace.Length + 1, out line, out column);
						editor.CursorPosition = position + nspace.Length + 1;
						
						ResolveResult typeResult;
						INode item;
						CurrentRefactoryOperationsHandler.GetItem (document.Dom, document, editor, out typeResult, out item);
						if (typeResult != null && !(typeResult is MethodResolveResult) && typeResult.ResolvedType != null) 
							yield return new MemberReference(null, filePath, position, line, column, nspace, nspace);
					}
				}
			}
		}
	}
}

