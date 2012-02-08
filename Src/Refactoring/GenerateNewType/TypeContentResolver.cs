using System;
using System.Globalization;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public interface IResolveTypeContent
	{
		string GetNewTypeContent(string typeName, string indent, string eol);
		string GetNewTypeFileContent(string typeContent, string nspace, string eol);
	}
	
	internal delegate string FileFormatResolving(string typeName, string indent, string eol);
	
	public class TypeContentResolver : IResolveTypeContent
	{
		static string fileFormat= @"namespace {{0}} {{{{{0}}}}}";
		static string interfaceFileFormat = "<indent>public interface {0} {{<eol><indent>\t<eol><indent>}}";
		static string classFormat = "<indent>public class {0} {{<eol><indent>\t<eol><indent>}}";
		
		static FileFormatResolving interfaceFormat = (name, indent, eol) => {
			if (name.Length > 1 && name[0] == 'I' && char.GetUnicodeCategory(name[1]) == UnicodeCategory.UppercaseLetter) {
				var ifrmt = interfaceFileFormat.Replace ("<indent>", indent);
				return ifrmt.Replace ("<eol>", eol);
			}
			return null;
		};
		
		static FileFormatResolving defaultClassFormat = (name, indent, eol) => {
			var clsfrmt = classFormat.Replace("<indent>", indent);
			clsfrmt = clsfrmt.Replace("<eol>", eol);
			return clsfrmt;
		};
		
		FileFormatResolving[] resolvings = new []{interfaceFormat};
		
		public string GetNewTypeFileContent(string typeContent, string nspace, string eol){
			typeContent = eol + typeContent + eol;
			typeContent = typeContent.Replace("{", "{{").Replace("}", "}}");
			var preNspace = fileFormat.ToFormat(typeContent);
			return preNspace.ToFormat (nspace);
		}
		
		public string GetNewTypeContent (string typeName, string indent, string eol)
		{
			var typeFormat = GetTypeFormat (typeName, indent, eol);
			return typeFormat.ToFormat(typeName);
		}

		string GetTypeFormat (string typeName, string indent, string eol)
		{
			if (typeName == null) throw new ArgumentNullException("typeName", "new type name cannot be null");
			string format = null;
			foreach (var resolvingAttempt in resolvings) {
				format = resolvingAttempt(typeName, indent, eol);
				if (format != null) return format;
			}
			
			return defaultClassFormat(typeName, indent, eol);
		}
	}	
}

