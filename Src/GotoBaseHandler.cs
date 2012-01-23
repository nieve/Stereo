using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide;
using System;

namespace MonoDevelop.Stereo
{
	public class GotoBaseHandler : CommandHandler
	{
		
		protected override void Update (CommandInfo info)
		{
			Document doc = IdeApp.Workbench.ActiveDocument;
    		info.Enabled = true; //doc != null && doc.GetContent<IEditableTextBuffer> () != null;
			//base.Update (info);
		}
		
		protected override void Run (object data)
		{
			Document doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.FileName == FilePath.Null || IdeApp.ProjectOperations.CurrentSelectedSolution == null)
				return;
			ITextBuffer editor = doc.GetContent<ITextBuffer> ();
			if (editor == null)
				return;
			int line, column;
			editor.GetLineColumnFromPosition (editor.CursorPosition, out line, out column);
			ProjectDom ctx = doc.Dom;
			
			ResolveResult resolveResult;
			INode item;
			CurrentRefactoryOperationsHandler.GetItem (ctx, doc, editor, out resolveResult, out item);
			item = item ?? (resolveResult != null ? 
				(resolveResult.CallingMember != null ? resolveResult.CallingMember.DeclaringType : resolveResult.CallingType) : null);
			if (resolveResult == null) return;
			if (item == null)  item = (INode)resolveResult.ResolvedType.Type;
			
			//NOTE: for generate class- if resolveResult.ResolvedType.Type == null && resolveResult.ResolvedExpression != null
			// continue with Dodo...
			
			IType cls = item as IType;
				if (cls != null && cls.BaseTypes != null) {
					foreach (IReturnType bc in cls.BaseTypes) {
						IType bcls = ctx.GetType (bc);
						if (bcls != null && bcls.ClassType != ClassType.Interface && !bcls.Location.IsEmpty) {
							IdeApp.Workbench.OpenDocument (bcls.CompilationUnit.FileName, bcls.Location.Line, 
                            	bcls.Location.Column, OpenDocumentOptions.Default);
							return;
						}
					}
				return;
			}
			IMethod method = item as IMethod;
			if (method != null) {
				foreach (IReturnType bc in method.DeclaringType.BaseTypes) {
					IType bcls = ctx.GetType (bc);
					if (bcls != null && bcls.ClassType != ClassType.Interface && !bcls.Location.IsEmpty) {
						IMethod baseMethod = null;
						foreach (IMethod m in bcls.Methods) {
							if (m.Name == method.Name && m.Parameters.Count == m.Parameters.Count) {
								baseMethod = m;
								break;
							}
						}
						if (baseMethod != null)
							IdeApp.Workbench.OpenDocument (bcls.CompilationUnit.FileName, baseMethod.Location.Line, 
                            	baseMethod.Location.Column, OpenDocumentOptions.Default);
						return;
					}
				}
				return;
			}
		}
	}
}

