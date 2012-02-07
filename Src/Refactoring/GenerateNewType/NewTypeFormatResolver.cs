using System;
using System.Globalization;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public interface IResolveNewTypeFormat
	{
		string ResolveFileFormat(string typeName, string indent, string eol);
		string ResolveNewFile(string typeContent, string nspace);
	}
	
	internal delegate string FileFormatResolving(string typeName, string indent, string eol);
	
	public class NewTypeFormatResolver : IResolveNewTypeFormat
	{
		static string fileFormat= @"namespace {{0}} {{{{{0}}}}}";
		static string interfaceFileFormat = "<indent>public interface {1} {{<eol><indent>\t<eol><indent>}}";
		static string classFormat = "<indent>public class {1} {{<eol><indent>\t<eol><indent>}}";
		
		static FileFormatResolving interfaceFormat = (name, indent, eol) => {
			var ifrmt = interfaceFileFormat.Replace("<indent>", indent);
			if (name.Length > 1 && name[0] == 'I' && char.GetUnicodeCategory(name[1]) == UnicodeCategory.UppercaseLetter)
				return ifrmt.Replace("<eol>", eol);
			return null;
		};
		
		FileFormatResolving[] resolvings = new []{interfaceFormat};
		
		public string ResolveNewFile(string typeContent, string nspace){
			typeContent = typeContent.Replace("{", "{{").Replace("}", "}}");
			var preNspace = fileFormat.ToFormat(typeContent);
			return preNspace.ToFormat (nspace);
		}
		
		//TODO: Test!
		public string ResolveFileFormat (string typeName, string indent, string eol)
		{
			if (typeName == null) throw new ArgumentNullException("typeName", "new type name cannot be null");
			string format = null;
			foreach (var resolvingAttempt in resolvings) {
				format = resolvingAttempt(typeName, indent, eol);
				if (format != null) return format;
			}
			
			return defaultClassFormat(typeName, indent, eol);
		}
		
		static FileFormatResolving defaultClassFormat = (name, indent, eol) => {
			var clsfrmt = classFormat.Replace("<indent>", indent);
			clsfrmt = clsfrmt.Replace("<eol>", eol);
			return clsfrmt;
		};
	}	
}

