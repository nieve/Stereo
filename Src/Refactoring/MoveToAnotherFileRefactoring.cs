using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MonoDevelop.Ide;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;

namespace MonoDevelop.Stereo.Refactoring
{
	public class MoveToAnotherFileRefactoring : RefactoringOperation
	{
		IParseDocument docParser;
		IResolveTypeContent fileFormatResolver;
		
		public MoveToAnotherFileRefactoring ()
		{
			docParser = new DocumentParser();
			fileFormatResolver = new TypeContentResolver();
		}
		
		public MoveToAnotherFileRefactoring (IParseDocument provider, IResolveTypeContent resolver)
		{
			docParser = provider;
			fileFormatResolver = resolver;
		}
		
		public override bool IsValid(RefactoringOptions options)
		{
			if (docParser.IsCurrentPositionTypeDeclarationUnmatchingFileName()) {
				var types = docParser.GetTypes ();
				if (types == null)
					return false;
				return types.Count () > 1;
			}
			return false;
		}
		
		public override void Run (RefactoringOptions options)
		{
			MessageService.ShowCustomDialog((Dialog) new QuickFixDialog(options, this));
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
			textReplaceChange.FileName = docParser.GetCurrentFilePath();
			textReplaceChange.RemovedChars = contentLength + 1;
			int num = editor.Document.LocationToOffset(body.Start.Line, 1);
			textReplaceChange.Offset = num - 1;
			textReplaceChange.InsertedText = string.Empty;
			changes.Add (textReplaceChange);
			
			return changes;
		}
	}
}

