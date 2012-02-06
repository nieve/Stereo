using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Ide;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring
{
	public class MoveToAnotherFileRefactoring : RefactoringOperation
	{
		IParseDocument docParser;
		
		public MoveToAnotherFileRefactoring ()
		{
			docParser = new DocumentParser();
		}
		
		public MoveToAnotherFileRefactoring (IParseDocument provider)
		{
			docParser = provider;
		}
		
		public override bool IsValid(RefactoringOptions options)
		{
			var types = docParser.GetTypes();
			if (types == null) return false;
			if (types.Count() > 1 && docParser.IsCurrentPositionFileNameType())
				return true;
			return false;
		}
		
		public override void Run (RefactoringOptions options)
		{
			Console.WriteLine("Ran!!!!!!!!!!");
//			var declaringType = options.ResolveResult.CallingType;
//			MonoDevelop.Ide.Gui.Document doc = options.Document;
//			var fileName = doc.FileName;
//			MonoDevelop.Ide.Gui.Document openDocument = IdeApp.Workbench.OpenDocument(fileName, (OpenDocumentOptions) 39);
//			if (openDocument == null) {
//				MessageService.ShowError(string.Format("Can't open file {0}.", fileName));
//			}
//			else {
//				data = openDocument.Editor;
//				if (data == null) return;
//				insertionPoint = GetInsertionPoint(openDocument, declaringType);
//				base.Run(options);
//			}
		}
	}
}

