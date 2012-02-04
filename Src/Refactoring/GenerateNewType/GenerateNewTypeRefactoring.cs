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
			int num = data.LocationToOffset (domLocation.Line - 1, domLocation.Column);
			if (num < 0) return list;
			DocumentLocation documentLocation = data.OffsetToLocation (num);
			list.Add (GetInsertionPosition (data.Document, documentLocation.Line, documentLocation.Column));
			list [0].LineBefore = NewLineInsertion.None;
			List<InsertionPoint> result;
			
			domRegion = type.BodyRegion;
			domLocation = domRegion.End;
			num = data.LocationToOffset (domLocation.Line, domLocation.Column);
			while (num < data.Length && data.GetCharAt(num) != '}') {
				num++;
			}
			documentLocation = data.OffsetToLocation (num);
			
			LineSegment lineAfterClassEnd = data.GetLine (documentLocation.Line + 1);
			NewLineInsertion lineBefore;
			if (lineAfterClassEnd != null && lineAfterClassEnd.EditableLength == lineAfterClassEnd.GetIndentation (data.Document).Length)
				lineBefore = NewLineInsertion.None;
			else
				lineBefore = NewLineInsertion.BlankLine;
			
			if (lineBefore == NewLineInsertion.Eol) {
				list.Add(new InsertionPoint (new DocumentLocation (documentLocation.Line + 1, documentLocation.Column - 1), lineBefore, NewLineInsertion.Eol));
			}
			else {
				list.Add(new InsertionPoint (new DocumentLocation (documentLocation.Line, documentLocation.Column), lineBefore, NewLineInsertion.Eol));
			}
			
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
		
		private static void CheckEndPoint (Mono.TextEditor.Document doc, InsertionPoint point, bool isStartPoint)
		{
			DocumentLocation location = point.Location;
			LineSegment line = doc.GetLine (location.Line);
			if (line != null)
			{
				int arg_3F_0 = doc.GetLineIndent (line).Length + 1;
				location = point.Location;
				if (arg_3F_0 < location.Column)
				{
					point.LineBefore = NewLineInsertion.BlankLine;
				}
				location = point.Location;
				if (location.Column < line.EditableLength + 1)
				{
					point.LineAfter = NewLineInsertion.Eol;
				}
			}
		}
		
		private static void CheckStartPoint (Mono.TextEditor.Document doc, InsertionPoint point, bool isEndPoint)
		{
			DocumentLocation location = point.Location;
			LineSegment line = doc.GetLine (location.Line);
			if (line != null)
			{
				int arg_42_0 = doc.GetLineIndent (line).Length + 1;
				location = point.Location;
				if (arg_42_0 == location.Column)
				{
					location = point.Location;
					int num = location.Line;
					while (num > 1 && doc.GetLineIndent (num - 1).Length == doc.GetLine (num - 1).EditableLength)
					{
						num--;
					}
					line = doc.GetLine (num);
					point.Location = new DocumentLocation (num, doc.GetLineIndent (line).Length + 1);
				}
				int arg_CC_0 = doc.GetLineIndent (line).Length + 1;
				location = point.Location;
				if (arg_CC_0 < location.Column)
				{
					point.LineBefore = NewLineInsertion.Eol;
				}
				location = point.Location;
				if (location.Column < line.EditableLength + 1)
				{
					point.LineAfter = (isEndPoint ? NewLineInsertion.Eol : NewLineInsertion.BlankLine);
				}
			}
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
			var content = fileFormat.ToFormat(nspace, newTypeName);
			textReplaceChange.InsertedText = content;
			
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

