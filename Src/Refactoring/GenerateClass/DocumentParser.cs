using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring.GenerateClass
{
	public interface IParseDocument{
		MemberResolveResult GetResolvedTypeNameResult ();
		FilePath GetCurrentFilePath();
	}
	
	public class DocumentParser : IParseDocument
	{
		public FilePath GetCurrentFilePath(){
			Document doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null) return null;
			return doc.FileName;
		}
		
		public MemberResolveResult GetResolvedTypeNameResult ()
		{
			Document doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.FileName == FilePath.Null || IdeApp.ProjectOperations.CurrentSelectedSolution == null)
				return null;
			ITextBuffer editor = doc.GetContent<ITextBuffer> ();
			if (editor == null)
				return null;
			int line, column;
			editor.GetLineColumnFromPosition (editor.CursorPosition, out line, out column);
			ProjectDom ctx = doc.Dom;
			
			ResolveResult resolveResult;
			INode item;
			CurrentRefactoryOperationsHandler.GetItem (ctx, doc, editor, out resolveResult, out item);
			
			return (resolveResult is MemberResolveResult) ?
				(MemberResolveResult)resolveResult : null;
		}
	}
}

