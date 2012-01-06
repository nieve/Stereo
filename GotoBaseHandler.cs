//using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide;

namespace Stereo
{
	public class GotoBaseHandler : CommandHandler
	{
		public GotoBaseHandler ()
		{
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
			IMember eitem = resolveResult != null ? (resolveResult.CallingMember ?? resolveResult.CallingType) : null;
			string itemName = null;
			if (item is IMember && !(item is IType))
			itemName = ((IMember)item).FullName;
			if (item != null && eitem != null && 
			    (eitem.Equals (item) || (eitem.FullName == itemName && !(eitem is IProperty) && !(eitem is IField) && !(eitem is IMethod)))) {
				item = eitem;
				eitem = null;
			}
			
			IType eclass = null;
			if (item is IType) {
				if (((IType)item).ClassType == ClassType.Interface)
					eclass = null; //CurrentRefactoryOperationsHandler.FindEnclosingClass (ctx, editor.Name, line, column); 
				else
					eclass = (IType)item;
				if (eitem is IMethod && ((IMethod)eitem).IsConstructor && eitem.DeclaringType.Equals (item)) {
					item = eitem;
					eitem = null;
				}
			}
			Refactorer refactorer = new Refactorer (ctx, doc.CompilationUnit, eclass, item, null);
			refactorer.GoToDeclaration ();
		}
	}
}

