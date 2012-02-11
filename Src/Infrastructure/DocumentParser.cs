using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.TextEditor;

namespace MonoDevelop.Stereo
{
	public interface IParseDocument{
		MemberResolveResult GetResolvedTypeNameResult ();
		FilePath GetCurrentFilePath();
		IEnumerable<IType> GetTypes();
		bool IsCurrentPositionTypeDeclarationUnmatchingFileName();
		DuplicateText GetTextToDuplicate ();
		bool ActiveDocumentAndEditorExist();
		void AppendDuplicatedText(DuplicateText text);
	}
	
	// TODO: Refactor to respect SRP
	public class DocumentParser : IParseDocument {
		MonoDevelop.Ide.Gui.Document activeDocument = null;
		ITextBuffer data = null;
		
		public IEnumerable<IType> GetTypes ()
		{
			Document doc = GetActiveDocument ();
			if (doc == null || doc.FileName == FilePath.Null || doc.Dom == null)
				return null;
			return doc.Dom.GetTypes(doc.FileName);
		}
		
		public FilePath GetCurrentFilePath(){
			Document doc = GetActiveDocument();
			if (doc == null) return null;
			return doc.FileName;
		}
		
		public bool IsCurrentPositionTypeDeclarationUnmatchingFileName() {
			var res = GetResolvedResult();
			if (res != null) {
				return res.CallingMember == null && res.ResolvedType != null 
					&& res.ResolvedType.Name != GetActiveDocument().FileName.FileNameWithoutExtension;
			}
			return false;
		}
		
		public MemberResolveResult GetResolvedTypeNameResult ()
		{
			ResolveResult resolveResult = GetResolvedResult ();
			
			return (resolveResult is MemberResolveResult) ?
				(MemberResolveResult)resolveResult : null;
		}
		
		public DuplicateText GetTextToDuplicate ()
		{
			MonoDevelop.Ide.Gui.Document doc = GetActiveDocument();
			if (doc == null) new EmptyDuplicateText();
			var data = GetData();
			var editor = doc.Editor;
			if (editor.IsSomethingSelected) {				
				return new SelectedDuplicateText(editor.SelectedText, editor.SelectionRange.EndOffset);
			}
			
			int line, column;
			data.GetLineColumnFromPosition (data.CursorPosition, out line, out column);
			var offset = data.GetPositionFromLineColumn(line + 1, 1);
			return new LineDuplicateText(editor.GetLineText(line), offset, editor.EolMarker);
		}
		
		public bool ActiveDocumentAndEditorExist ()
		{
			MonoDevelop.Ide.Gui.Document activeDocument = GetActiveDocument();
			return (activeDocument != null || activeDocument.Editor != null);
		}
		
		public void AppendDuplicatedText(DuplicateText text){
			var doc = GetActiveDocument();
			doc.Editor.Insert(text.Offset, text);
		}
		
		private Document GetActiveDocument ()
		{
			return activeDocument ?? IdeApp.Workbench.ActiveDocument;
		}

		private ITextBuffer GetData (Document doc)
		{
			return data ?? doc.GetContent<ITextBuffer> ();
		}

		private ITextBuffer GetData ()
		{
			return GetData(GetActiveDocument());
		}

		private ResolveResult GetResolvedResult ()
		{
			Document doc = GetActiveDocument();
			if (doc == null || doc.FileName == FilePath.Null || IdeApp.ProjectOperations.CurrentSelectedSolution == null)
				return null;
			ITextBuffer data = GetData (doc);
			if (data == null)
				return null;
			int line, column;
			data.GetLineColumnFromPosition (data.CursorPosition, out line, out column);
			ProjectDom ctx = doc.Dom;
			
			ResolveResult resolveResult;
			INode item;
			CurrentRefactoryOperationsHandler.GetItem (ctx, doc, data, out resolveResult, out item);
			return resolveResult;
		}
	}
}

