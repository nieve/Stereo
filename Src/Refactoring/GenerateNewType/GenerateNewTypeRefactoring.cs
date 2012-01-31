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
						indent = data.Document.GetLine(/*declaringType.Location.Line*/ 45).GetIndentation(data.Document) ?? "";
					}
					catch {
						indent = "";
					}
					indent += "\t";
					List<InsertionPoint> insertionPoints = new List<InsertionPoint>(); //CodeGenerationService.GetInsertionPoints (openDocument, declaringType);
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

