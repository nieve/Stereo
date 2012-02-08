using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Refactoring;
using System.Collections.Generic;
using System;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public interface IParseDocument{
		MemberResolveResult GetResolvedTypeNameResult ();
		FilePath GetCurrentFilePath();
		IEnumerable<IType> GetTypes();
		bool IsCurrentPositionNotFileNameType();
	}
	
	public class DocumentParser : IParseDocument {
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

		static Document GetActiveDocument ()
		{
			return IdeApp.Workbench.ActiveDocument;
		}
		
		public bool IsCurrentPositionNotFileNameType() {
			var res = GetResolvedResult();
			if (res != null) {
				return res.CallingMember == null && res.ResolvedType != null 
					&& res.ResolvedType.Name != GetActiveDocument().FileName.FileNameWithoutExtension;
			}
			return false;
		}

		private ITextBuffer GetEditor (Document doc)
		{
			return doc.GetContent<ITextBuffer> ();
		}

		private ITextBuffer GetEditor ()
		{
			return GetActiveDocument().GetContent<ITextBuffer> ();
		}

		private ResolveResult GetResolvedResult ()
		{
			Document doc = GetActiveDocument();
			if (doc == null || doc.FileName == FilePath.Null || IdeApp.ProjectOperations.CurrentSelectedSolution == null)
				return null;
			ITextBuffer editor = GetEditor (doc);
			if (editor == null)
				return null;
			int line, column;
			editor.GetLineColumnFromPosition (editor.CursorPosition, out line, out column);
			ProjectDom ctx = doc.Dom;
			
			ResolveResult resolveResult;
			INode item;
			CurrentRefactoryOperationsHandler.GetItem (ctx, doc, editor, out resolveResult, out item);
			return resolveResult;
		}
		
		public MemberResolveResult GetResolvedTypeNameResult ()
		{
			ResolveResult resolveResult = GetResolvedResult ();
			
			return (resolveResult is MemberResolveResult) ?
				(MemberResolveResult)resolveResult : null;
		}
	}
}

