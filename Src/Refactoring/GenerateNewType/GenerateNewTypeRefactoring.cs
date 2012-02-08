using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Projects.Dom;
using System.Collections.Generic;
using Mono.TextEditor;
using Mono.TextEditor.PopupWindow;
using Gtk;
using System.Linq;
using System.Text;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public class GenerateNewTypeRefactoring : RefactoringOperation
	{
		IParseDocument docParser;
		IResolveTypeContent fileFormatResolver;
		string indent = "";
		InsertionPoint insertionPoint = null;
		TextEditorData data = null;

		public TextEditorData Data {
			get {
				return this.data;
			}
			set {
				data = value;
			}
		}

		public InsertionPoint InsertionPoint {
			get {
				return this.insertionPoint;
			}
			set {
				insertionPoint = value;
			}
		}
		
		public GenerateNewTypeRefactoring ()
		{
			this.Name = "Generate new type";
			docParser = new DocumentParser();
			fileFormatResolver = new TypeContentResolver();
		}
		
		public GenerateNewTypeRefactoring (IParseDocument provider, IResolveTypeContent resolver)
		{
			this.Name = "Generate new type";
			docParser = provider;
			fileFormatResolver = resolver;
		}
		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "Generate new _type";
	    }
		
		public override bool IsValid(RefactoringOptions options)
		{
			MemberResolveResult resolvedTypeName = docParser.GetResolvedTypeNameResult();
			return resolvedTypeName != null && resolvedTypeName.ResolvedMember == null 
				&& resolvedTypeName.ResolvedExpression != null && resolvedTypeName.ResolvedType.Type == null;
		}
		
		public override void Run (RefactoringOptions options)
		{
			var declaringType = options.ResolveResult.CallingType;
			MonoDevelop.Ide.Gui.Document doc = options.Document;
			var fileName = doc.FileName;
			MonoDevelop.Ide.Gui.Document openDocument = IdeApp.Workbench.OpenDocument(fileName, (OpenDocumentOptions) 39);
			if (openDocument == null) {
				MessageService.ShowError(string.Format("Can't open file {0}.", fileName));
			}
			else {
				insertionPoint = GetInsertionPoint(openDocument, declaringType);
				base.Run(options);
			}
		}
		
		private InsertionPoint GetInsertionPoint (MonoDevelop.Ide.Gui.Document document, IType type)
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
				
		public override List<Change> PerformChanges (RefactoringOptions options, object properties)
		{
			List<Change> changes = new List<Change>();
			var textReplaceChange = new TextReplaceChange();
			textReplaceChange.FileName = docParser.GetCurrentFilePath();
			textReplaceChange.RemovedChars = 0;
			int num = data.Document.LocationToOffset(insertionPoint.Location);
			textReplaceChange.Offset = num;
			
			var resolveResult = docParser.GetResolvedTypeNameResult ();
			if (resolveResult == null) throw new InvalidOperationException("Cannot generate class here");
			var nspace = resolveResult.CallingType.Namespace;
			string newTypeName = resolveResult.ResolvedExpression.Expression;
			StringBuilder contentBuilder = new StringBuilder();
			if (insertionPoint.LineBefore == NewLineInsertion.Eol) contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			var content = fileFormatResolver.GetNewTypeContent(newTypeName, indent, data.EolMarker);
			contentBuilder.Append(content);
			if (insertionPoint.LineAfter == NewLineInsertion.None) contentBuilder.Append(data.EolMarker);
			textReplaceChange.InsertedText = contentBuilder.ToString();
			
			changes.Add(textReplaceChange);
			return changes;
		}
	}
}

