using System;
using MonoDevelop.Projects.Dom;

namespace MonoDevelop.Stereo
{
	public class NonConcreteTypeContext : DocumentContext, INonConcreteTypeContext
	{
		public bool IsCurrentLocationNonConcreteType ()
		{
			var result = GetResolvedResult();
			if (result.ResolvedType != null) {
				var type = result.ResolvedType.Type;
				if (type != null) return !type.IsAbstract && type.MemberType == MonoDevelop.Projects.Dom.MemberType.Type;
			}
			return false;
		}

		public MonoDevelop.Projects.Dom.IType GetNonConcreteType ()
		{
			var result = GetResolvedResult();
			return result.ResolvedType.Type;
		}
	}
	
	public interface INonConcreteTypeContext : IDocumentContext
	{
		bool IsCurrentLocationNonConcreteType();
		IType GetNonConcreteType();
	}
}

