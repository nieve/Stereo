using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Projects.Dom;
using System.Collections.Generic;
using MonoDevelop.Stereo;

namespace MonoDevelop.Stereo.Refactoring.GenerateClass
{
	public class GenerateClassRefactoring : RefactoringOperation
	{
		ResolvedTypeNameResultProvider typeNameProvider;
		public GenerateClassRefactoring ()
		{
			this.Name = "Generate Class";
			typeNameProvider = new ResolvedTypeNameResultProvider();
		}
		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "Generate _Class";
	    }
		
		public override bool IsValid(RefactoringOptions options)
		{
			MemberResolveResult resolvedTypeName = typeNameProvider.GetResolvedTypeNameResult();
			return resolvedTypeName != null && resolvedTypeName.ResolvedMember == null 
				&& resolvedTypeName.ResolvedExpression != null && resolvedTypeName.ResolvedType.Type == null;
		}

		public override List<Change> PerformChanges (RefactoringOptions options, object properties)
		{
			var resolveResult = typeNameProvider.GetResolvedTypeNameResult ();
			if (resolveResult == null) throw new InvalidOperationException("Cannot generate class here");
			
			List<Change> changes = new List<Change>();
			
			var nspace = resolveResult.CallingType.Namespace;
			var fileFormat = "namespace {0} {{\r\n\tpublic class {1}{{\r\n\t\t\r\n\t}}\r\n}}";
			string className = resolveResult.ResolvedExpression.Expression;
			var content = fileFormat.ToFormat(nspace, className);
			CreateFileChange createFileChange = new CreateFileChange("{0}.cs".ToFormat(className), content);
			changes.Add(createFileChange);
			
			return changes;
		}
	}
}

