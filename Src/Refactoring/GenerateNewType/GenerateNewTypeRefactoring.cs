using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Projects.Dom;
using System.Collections.Generic;

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

