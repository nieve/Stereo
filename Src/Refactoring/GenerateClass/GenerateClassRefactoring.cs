using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Projects.Dom;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.Refactoring.GenerateClass
{
	public class GenerateClassRefactoring : RefactoringOperation
	{
		IParseDocument docParser;
		
		public GenerateClassRefactoring ()
		{
			this.Name = "Generate Class";
			docParser = new DocumentParser();
		}
		
		public GenerateClassRefactoring (IParseDocument provider)
		{
			this.Name = "Generate Class";
			docParser = provider;
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
			var fileFormat = "namespace {0} {{\r\n\tpublic class {1}{{\r\n\t\t\r\n\t}}\r\n}}";
			string className = resolveResult.ResolvedExpression.Expression;
			var content = fileFormat.ToFormat(nspace, className);
			CreateFileChange createFileChange = new CreateFileChange(@"{0}\{1}.cs".ToFormat(currentDir, className), content);
			changes.Add(createFileChange);
			
			return changes;
		}
	}
}

