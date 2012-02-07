using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Ide;
using System.Linq;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Refactoring
{
	public class MoveToAnotherFileRefactoring : RefactoringOperation
	{
		IParseDocument docParser;
		IResolveNewTypeFormat fileFormatResolver;
		
		public MoveToAnotherFileRefactoring ()
		{
			docParser = new DocumentParser();
			fileFormatResolver = new NewTypeFormatResolver();
		}
		
		public MoveToAnotherFileRefactoring (IParseDocument provider, IResolveNewTypeFormat resolver)
		{
			docParser = provider;
			fileFormatResolver = resolver;
		}
		
		//TODO: Test!
		public override bool IsValid(RefactoringOptions options)
		{
			var types = docParser.GetTypes();
			if (types == null) return false;
			if (types.Count() > 1 && docParser.IsCurrentPositionFileNameType())
				return true;
			return false;
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
			content = fileFormatResolver.ResolveNewFile(editor.EolMarker + content + editor.EolMarker, nspace);
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

