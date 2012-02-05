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
		IResolveNewTypeFileFormat fileFormatResolver;
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

		public IParseDocument DocParser {
			get {
				return this.docParser;
			}
			set {
				docParser = value;
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
			this.Name = "Generate Class";
			docParser = new DocumentParser();
			fileFormatResolver = new NewTypeFileFormatResolver();
		}
		
		public GenerateNewTypeRefactoring (IParseDocument provider, IResolveNewTypeFileFormat resolver)
		{
			this.Name = "Generate Class";
			docParser = provider;
			fileFormatResolver = resolver;
		}
		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "Generate _Class";
	    }
		
		public override bool IsValid(RefactoringOptions options)
		{
			MemberResolveResult resolvedTypeName = docParser.GetResolvedTypeNameResult();
			return resolvedTypeName != null && resolvedTypeName.ResolvedMember == null 
				&& resolvedTypeName.ResolvedExpression != null && resolvedTypeName.ResolvedType.Type == null;
		}
		
		public override void Run (RefactoringOptions options)
		{
			//TODO: need to get declaringType & look into CodeGenerationService.GetInsertionPoints Ln.68
			var declaringType = options.ResolveResult.CallingType;
			MonoDevelop.Ide.Gui.Document doc = options.Document;
			var fileName = doc.FileName;
			MonoDevelop.Ide.Gui.Document openDocument = IdeApp.Workbench.OpenDocument(fileName, (OpenDocumentOptions) 39);
			if (openDocument == null) {
				MessageService.ShowError(string.Format("Can't open file {0}.", fileName));
			}
			else {
				data = openDocument.Editor;
				if (data == null) return;
				openDocument.RunWhenLoaded((System.Action) (() => {
					try {
						indent = data.Document.GetLine(declaringType.Location.Line).GetIndentation(data.Document) ?? "";
					}
					catch {
						indent = "";
					}
					indent += "\t";
					
					List<InsertionPoint> insertionPoints = GetInsertionPoints (openDocument, declaringType);
					InsertionCursorEditMode insertion = new InsertionCursorEditMode(data.Parent, insertionPoints);
					for (int i = 0; i < insertion.InsertionPoints.Count && insertion.InsertionPoints[i].Location < data.Caret.Location; ++i)
						insertion.CurIndex = i;
					insertion.HelpWindow = new ModeHelpWindow() {
						TransientFor = (Window) IdeApp.Workbench.RootWindow,
						TitleText = GettextCatalog.GetString("<b>Create Method -- Targeting</b>"),
						Items = {
							new KeyValuePair<string, string>(GettextCatalog.GetString("<b>Key</b>"), GettextCatalog.GetString("<b>Behavior</b>")),
							new KeyValuePair<string, string>(GettextCatalog.GetString("<b>Up</b>"), GettextCatalog.GetString("Move to <b>previous</b> target point.")),
							new KeyValuePair<string, string>(GettextCatalog.GetString("<b>Down</b>"), GettextCatalog.GetString("Move to <b>next</b> target point.")),
							new KeyValuePair<string, string>(GettextCatalog.GetString("<b>Enter</b>"), GettextCatalog.GetString("<b>Declare new method</b> at target point.")),
							new KeyValuePair<string, string>(GettextCatalog.GetString("<b>Esc</b>"), GettextCatalog.GetString("<b>Cancel</b> this refactoring."))
						}
					};
					insertion.StartMode();
					insertion.Exited += (EventHandler<InsertionCursorEventArgs>) ((s, args) => {
						if (!args.Success) return;
						SetInsertionPoint(args.InsertionPoint);
						base.Run(options);
						if (string.IsNullOrEmpty(fileName)) return;
						data.ClearSelection();
//						data.Caret.Offset = selectionEnd;
//						data.SetSelection(this.selectionStart, this.selectionEnd);
					});
				}));
			}
		}
		
		public void SetInsertionPoint(InsertionPoint point)
		{
			insertionPoint = point;
		}
		
		private List<InsertionPoint> GetInsertionPoints(MonoDevelop.Ide.Gui.Document document, IType type){
			var data = document.Editor;
			var parsedDocument = document.ParsedDocument;
			if (data == null)
			{
				throw new System.ArgumentNullException ("data");
			}
			if (parsedDocument == null)
			{
				throw new System.ArgumentNullException ("parsedDocument");
			}
			if (type == null)
			{
				throw new System.ArgumentNullException ("type");
			}
			type = (parsedDocument.CompilationUnit.GetTypeAt (type.Location) ?? type);
			List<InsertionPoint> list = new List<InsertionPoint> ();
			DomRegion domRegion = type.BodyRegion;
			DomLocation domLocation = domRegion.Start;
			int num = data.LocationToOffset (domLocation.Line - 1, 1);
			if (num < 0) return list;
			LineSegment lineBeforeClass = data.GetLine (domLocation.Line - 1);
			NewLineInsertion lineBefore;
			DocumentLocation documentLocation;
			if (lineBeforeClass != null && lineBeforeClass.EditableLength == lineBeforeClass.GetIndentation (data.Document).Length) {
				lineBefore = NewLineInsertion.BlankLine;
				documentLocation = data.OffsetToLocation (num);
			}
			else {
				lineBefore = NewLineInsertion.Eol;
				num = data.LocationToOffset (domLocation.Line, 1);
				documentLocation = data.OffsetToLocation (num);
			}
//			list.Add (GetInsertionPosition (data.Document, documentLocation.Line, documentLocation.Column));
			list.Add(new InsertionPoint (documentLocation, lineBefore, NewLineInsertion.None));
//			list [0].LineBefore = NewLineInsertion.None;
			List<InsertionPoint> result;
			
			domRegion = type.BodyRegion;
			domLocation = domRegion.End;
			num = data.LocationToOffset (domLocation.Line, 1);
			while (num < data.Length && data.GetCharAt(num) != '}') {
				num++;
			}
			num++;
			documentLocation = data.OffsetToLocation (num);
			
			LineSegment lineAfterClassEnd = data.GetLine (domLocation.Line + 1);
			NewLineInsertion lineAfter;
			if (lineAfterClassEnd != null && lineAfterClassEnd.EditableLength == lineAfterClassEnd.GetIndentation (data.Document).Length)
				lineAfter = NewLineInsertion.BlankLine;
			else
				lineAfter = NewLineInsertion.Eol;
			
//			if (lineBefore == NewLineInsertion.Eol)
//				list.Add(new InsertionPoint (new DocumentLocation (documentLocation.Line + 1, documentLocation.Column - 1), lineBefore, NewLineInsertion.Eol));
//
//			else
			list.Add(new InsertionPoint (documentLocation, NewLineInsertion.None, lineAfter));
			
			result = list;
			return result;
		}
		
		private static InsertionPoint GetInsertionPosition (Mono.TextEditor.Document doc, int line, int column)
		{
			int num = doc.LocationToOffset (line, column) + 1;
			LineSegment line2 = doc.GetLine (line);
			InsertionPoint result;
			if (line2 != null)
			{
				if (num < line2.Offset + line2.EditableLength)
				{
					result = new InsertionPoint (new DocumentLocation (line, column + 1), NewLineInsertion.Eol, NewLineInsertion.BlankLine);
					return result;
				}
			}
			LineSegment line3 = doc.GetLine (line + 1);
			if (line3 == null)
			{
				result = new InsertionPoint (new DocumentLocation (line, column + 1), NewLineInsertion.BlankLine, NewLineInsertion.BlankLine);
			}
			else
			{
				for (int i = line3.Offset; i < line3.Offset + line3.EditableLength; i++)
				{
					char charAt = doc.GetCharAt (i);
					if (!char.IsWhiteSpace (charAt))
					{
						result = new InsertionPoint (new DocumentLocation (line + 1, 1), NewLineInsertion.Eol, NewLineInsertion.BlankLine);
						return result;
					}
				}
				result = new InsertionPoint (new DocumentLocation (line + 1, 1), NewLineInsertion.Eol, NewLineInsertion.None);
			}
			return result;
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
			var fileFormat = fileFormatResolver.ResolveFileFormat(newTypeName, indent, data.EolMarker, false);
			StringBuilder contentBuilder = new StringBuilder();
			if (insertionPoint.LineBefore == NewLineInsertion.Eol) contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			var content = fileFormat.ToFormat(nspace, newTypeName);
			contentBuilder.Append(content);
			if (insertionPoint.LineAfter == NewLineInsertion.Eol) contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			textReplaceChange.InsertedText = contentBuilder.ToString();
			
			changes.Add(textReplaceChange);
//			var resolveResult = docParser.GetResolvedTypeNameResult ();
//			if (resolveResult == null) throw new InvalidOperationException("Cannot generate class here");
//				
//			var currentDir = docParser.GetCurrentFilePath().ParentDirectory;
//			var nspace = resolveResult.CallingType.Namespace;
//			string newTypeName = resolveResult.ResolvedExpression.Expression;
//			var fileFormat = fileFormatResolver.ResolveFileFormat(newTypeName);
//			var content = fileFormat.ToFormat(nspace, newTypeName);
//			CreateFileChange createFileChange = new CreateFileChange(@"{0}\{1}.cs".ToFormat(currentDir, newTypeName), content);
//			changes.Add(createFileChange);
			
			return changes;
		}
	}
}

