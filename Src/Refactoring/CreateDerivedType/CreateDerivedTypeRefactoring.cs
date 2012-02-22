using System;
using System.Collections.Generic;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using Mono.TextEditor;
using MonoDevelop.Projects.Dom;
using System.Text;

namespace MonoDevelop.Stereo.Refactoring.CreateDerivedType
{
	public interface ICreateDerivedTypeRefactoring : IRefactorTask {}
	
	public class CreateDerivedTypeRefactoring : AppendingNewTypeRefactoringOperation, ICreateDerivedTypeRefactoring
	{
		INonConcreteTypeContext context;
		InsertionPoint insertionPoint = null;
		IType type;
		IResolveTypeContent fileFormatResolver;
		
		public CreateDerivedTypeRefactoring ()
		{
			context = new NonConcreteTypeContext();
			fileFormatResolver = new TypeContentResolver();
		}		
		
		public CreateDerivedTypeRefactoring (INonConcreteTypeContext context, IResolveTypeContent resolver)
		{
			this.context = context;
			fileFormatResolver = resolver;
		}
		
		public override void Run (RefactoringOptions options)
		{
			type = context.GetNonConcreteType();
			MonoDevelop.Ide.Gui.Document doc = options.Document;
			var fileName = doc.FileName;
			MonoDevelop.Ide.Gui.Document openDocument = IdeApp.Workbench.OpenDocument(fileName, (OpenDocumentOptions) 39);
			if (openDocument == null) {
				MessageService.ShowError(string.Format("Can't open file {0}.", fileName));
			}
			else {
				insertionPoint = GetInsertionPoint(openDocument, type);
				base.Run(options);
			}
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object properties)
		{
			List<Change> changes = new List<Change>();
			var newTypeName = "Test : " + type.Name;
			var textReplaceChange = new TextReplaceChange();
			textReplaceChange.FileName = context.GetCurrentFilePath();
			textReplaceChange.RemovedChars = 0;
			int num = data.Document.LocationToOffset(insertionPoint.Location);
			textReplaceChange.Offset = num;
			
			StringBuilder contentBuilder = new StringBuilder();
			if (insertionPoint.LineBefore == NewLineInsertion.Eol) contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			
			// TODO: Add methods implementations & ask user for new concrete type name
			var content = fileFormatResolver.GetNewTypeContent(newTypeName, indent, data.EolMarker);
			contentBuilder.Append(content);
			
			if (insertionPoint.LineAfter == NewLineInsertion.None) contentBuilder.Append(data.EolMarker);
			textReplaceChange.InsertedText = contentBuilder.ToString();
			
			changes.Add(textReplaceChange);			
			return changes;
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			return IsValid ();
		}
		
		public bool IsValid ()
		{
			return context.IsCurrentLocationNonConcreteType();
		}
		
		public string Title  { get {return "Create derived type";}}
		public int Position { get {return 2;}}
	}
}