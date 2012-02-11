using System;
using MonoDevelop.Stereo.TextEditor;

namespace MonoDevelop.Stereo
{
	public interface ITextDuplicationContext : IDocumentContext
	{
		DuplicateText GetTextToDuplicate ();
		bool ActiveDocumentAndEditorExist();
		void AppendDuplicatedText(DuplicateText text);
	}
	
	public class TextDuplicationContext : DocumentContext, ITextDuplicationContext {
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
	}
}

