using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MonoDevelop.Ide;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using MonoDevelop.Stereo.Refactoring.QuickFixes;

namespace MonoDevelop.Stereo.Refactoring.MoveToAnotherFile
{
	public class MoveToAnotherFileRefactoring : RefactoringOperation, IRefactorTask
	{
		IMoveTypeContext context;
		IResolveTypeContent fileFormatResolver;
		
		public string Title{ get {return "Move to another file";}}
		
		public MoveToAnotherFileRefactoring ()
		{
			context = new MoveTypeContext();
			fileFormatResolver = new TypeContentResolver();
		}
		
		public MoveToAnotherFileRefactoring (IMoveTypeContext ctx, IResolveTypeContent resolver)
		{
			context = ctx;
			fileFormatResolver = resolver;
		}
		
		public bool IsValid() {
			if (context.IsCurrentPositionTypeDeclarationUnmatchingFileName()) {
				var types = context.GetTypes ();
				if (types == null)
					return false;
				return types.Count () > 1;
			}
			return false;
		}
		
		public override bool IsValid(RefactoringOptions options)
		{
			return IsValid();
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object properties)
		{
			List<Change> changes = new List<Change>();
			var resolveResult = options.ResolveResult;
			if (resolveResult == null) throw new InvalidOperationException("Cannot generate class here");
			var resType = resolveResult.ResolvedType;
			
			var doc = options.Document;
			var editor = doc.Editor;
			var currentDir = doc.FileName.ParentDirectory;
			var nspace = resType.Namespace;
			string typeName = resolveResult.ResolvedExpression.Expression;
			var body = resType.Type.BodyRegion;
			var content = editor.GetTextBetween(body.Start.Line, 1, body.End.Line, body.End.Column);
			var contentLength = content.Length;
			content = fileFormatResolver.GetNewTypeFileContent(content, nspace, editor.EolMarker);
			CreateFileChange createFileChange = new CreateFileChange(@"{0}\{1}.cs".ToFormat(currentDir, typeName), content);
			changes.Add(createFileChange);
			
			TextReplaceChange textReplaceChange = new TextReplaceChange();
			textReplaceChange.FileName = context.GetCurrentFilePath();
			textReplaceChange.RemovedChars = contentLength + 1;
			int num = editor.Document.LocationToOffset(body.Start.Line, 1);
			textReplaceChange.Offset = num - 1;
			textReplaceChange.InsertedText = string.Empty;
			changes.Add (textReplaceChange);
			
			return changes;
		}
	}
}

