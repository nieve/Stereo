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
				var data = openDocument.Editor;
				if (data == null) return;
				string indent;
				openDocument.RunWhenLoaded((System.Action) (() => {
					try {
						indent = data.Document.GetLine(declaringType.Location.Line).GetIndentation(data.Document) ?? "";
					}
					catch {
						indent = "";
					}
					indent += "\t";
					Console.WriteLine(declaringType.FullName);
					Console.WriteLine(declaringType.DeclaringType.FullName);
					List<InsertionPoint> insertionPoints = CodeGenerationService.GetInsertionPoints (openDocument, declaringType);
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
//						this.SetInsertionPoint(args.InsertionPoint);
						base.Run(options);
						if (string.IsNullOrEmpty(fileName)) return;
						data.ClearSelection();
//						data.Caret.Offset = selectionEnd;
//						data.SetSelection(this.selectionStart, this.selectionEnd);
					});
				}));
			}
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
			System.Collections.Generic.List<InsertionPoint> list = new System.Collections.Generic.List<InsertionPoint> ();
			DomRegion domRegion = type.BodyRegion;
			DomLocation domLocation = domRegion.Start;
			int arg_CA_1 = domLocation.Line;
			domRegion = type.BodyRegion;
			domLocation = domRegion.Start;
			int num = data.LocationToOffset (arg_CA_1, domLocation.Column);
			System.Collections.Generic.List<InsertionPoint> result;
			if (num < 0)
			{
				result = list;
			}
			else
			{
				while (num < data.Length && data.GetCharAt (num) != '{')
				{
					num++;
				}
				DocumentLocation documentLocation = data.OffsetToLocation (num);
				list.Add (GetInsertionPosition (data.Document, documentLocation.Line, documentLocation.Column));
				list [0].LineBefore = NewLineInsertion.None;
				foreach (IMember current in type.Members)
				{
					domRegion = current.BodyRegion;
					DomLocation end = domRegion.End;
					if (end.Line <= 0)
					{
						domLocation = current.Location;
						LineSegment line = data.GetLine (domLocation.Line);
						if (line == null)
						{
							continue;
						}
						domLocation = current.Location;
						end = new DomLocation (domLocation.Line, line.EditableLength + 1);
					}
					list.Add (GetInsertionPosition (data.Document, end.Line, end.Column));
				}
				list [list.Count - 1].LineAfter = NewLineInsertion.None;
				CheckStartPoint (data.Document, list [0], list.Count == 1);
				if (list.Count > 1)
				{
					list.RemoveAt (list.Count - 1);
					domRegion = type.BodyRegion;
					domLocation = domRegion.End;
					LineSegment line2 = data.GetLine (domLocation.Line - 1);
					NewLineInsertion lineBefore;
					if (line2 != null && line2.EditableLength == line2.GetIndentation (data.Document).Length)
					{
						lineBefore = NewLineInsertion.None;
					}
					else
					{
						lineBefore = NewLineInsertion.Eol;
					}
					domRegion = type.BodyRegion;
					domLocation = domRegion.End;
					LineSegment line3 = data.GetLine (domLocation.Line);
					domRegion = type.BodyRegion;
					domLocation = domRegion.End;
					int num2 = domLocation.Column - 1;
					while (num2 > 1 && char.IsWhiteSpace (data.GetCharAt (line3.Offset + num2 - 2)))
					{
						num2--;
					}
					System.Collections.Generic.List<InsertionPoint> arg_36F_0 = list;
					domRegion = type.BodyRegion;
					domLocation = domRegion.End;
					arg_36F_0.Add (new InsertionPoint (new DocumentLocation (domLocation.Line, num2), lineBefore, NewLineInsertion.Eol));
					CheckEndPoint (data.Document, list [list.Count - 1], list.Count == 1);
				}
				foreach (FoldingRegion current2 in parsedDocument.UserRegions.Where(r=>type.BodyRegion.Contains (r.Region)))
				{
					System.Collections.Generic.List<InsertionPoint> arg_3F1_0 = list;
					domRegion = current2.Region;
					domLocation = domRegion.Start;
					arg_3F1_0.Add (new InsertionPoint (new DocumentLocation (domLocation.Line + 1, 1), NewLineInsertion.Eol, NewLineInsertion.Eol));
					System.Collections.Generic.List<InsertionPoint> arg_41E_0 = list;
					domRegion = current2.Region;
					domLocation = domRegion.End;
					arg_41E_0.Add (new InsertionPoint (new DocumentLocation (domLocation.Line, 1), NewLineInsertion.Eol, NewLineInsertion.Eol));
					System.Collections.Generic.List<InsertionPoint> arg_44D_0 = list;
					domRegion = current2.Region;
					domLocation = domRegion.End;
					arg_44D_0.Add (new InsertionPoint (new DocumentLocation (domLocation.Line + 1, 1), NewLineInsertion.Eol, NewLineInsertion.Eol));
				}
				list.Sort ((InsertionPoint left, InsertionPoint right) => left.Location.CompareTo (right.Location));
				result = list;
			}
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
			var resolveResult = docParser.GetResolvedTypeNameResult ();
			if (resolveResult == null) throw new InvalidOperationException("Cannot generate class here");
			
			List<Change> changes = new List<Change>();
			
			var currentDir = docParser.GetCurrentFilePath().ParentDirectory;
			var nspace = resolveResult.CallingType.Namespace;
			string newTypeName = resolveResult.ResolvedExpression.Expression;
			var fileFormat = fileFormatResolver.ResolveFileFormat(newTypeName);
			var content = fileFormat.ToFormat(nspace, newTypeName);
			CreateFileChange createFileChange = new CreateFileChange(@"{0}\{1}.cs".ToFormat(currentDir, newTypeName), content);
			changes.Add(createFileChange);
			
			return changes;
		}
	}
}

