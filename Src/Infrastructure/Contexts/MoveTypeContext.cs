using System.Collections.Generic;
using System;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Core;

namespace MonoDevelop.Stereo
{
	public interface IMoveTypeContext : IDocumentContext
	{
		IEnumerable<IType> GetTypes();
		bool IsCurrentPositionTypeDeclarationUnmatchingFileName();		
	}
	
	public class MoveTypeContext : DocumentContext, IMoveTypeContext
	{
		public IEnumerable<IType> GetTypes ()
		{
			Document doc = GetActiveDocument ();
			if (doc == null || doc.FileName == FilePath.Null || doc.Dom == null)
				return null;
			return doc.Dom.GetTypes(doc.FileName);
		}
		
		public bool IsCurrentPositionTypeDeclarationUnmatchingFileName() {
			var res = GetResolvedResult();
			if (res != null) {
				return res.CallingMember == null && res.ResolvedType != null 
					&& res.ResolvedType.Name != GetActiveDocument().FileName.FileNameWithoutExtension;
			}
			return false;
		}
	}
}

