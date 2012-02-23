using System;
using MonoDevelop.Refactoring;
using Mono.TextEditor;
using MonoDevelop.Projects.Dom;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public class AppendingNewTypeRefactoringOperation : RefactoringOperation
	{
		protected TextEditorData data = null;
		protected string indent = "";
		
		protected InsertionPoint GetInsertionPoint (MonoDevelop.Ide.Gui.Document document, IType type)
		{
			data = document.Editor;
			if (data == null) {
				throw new System.ArgumentNullException ("data");
			}
			var parsedDocument = document.ParsedDocument;
			if (parsedDocument == null) {
				throw new System.ArgumentNullException ("parsedDocument");
			}
			if (type == null) {
				throw new System.ArgumentNullException ("type");
			}
			type = (parsedDocument.CompilationUnit.GetTypeAt (type.Location) ?? type);
			DomRegion domRegion = type.BodyRegion;
			var start = type.BodyRegion.Start.Line;
			indent = data.GetLine(start).GetIndentation(data.Document);
			DomLocation domLocation = domRegion.End;
			int num = data.LocationToOffset (domLocation.Line, 1);
			while (num < data.Length && data.GetCharAt(num) != '}') {
				num++;
			}
			num++;
			DocumentLocation documentLocation = data.OffsetToLocation (num);
			
			LineSegment lineAfterClassEnd = data.GetLine (domLocation.Line + 1);
			NewLineInsertion lineAfter;
			if (lineAfterClassEnd != null && lineAfterClassEnd.EditableLength == lineAfterClassEnd.GetIndentation (data.Document).Length)
				lineAfter = NewLineInsertion.BlankLine;
			else
				lineAfter = NewLineInsertion.None;
			
			return new InsertionPoint (documentLocation, NewLineInsertion.None, lineAfter);
		}
	}
}

