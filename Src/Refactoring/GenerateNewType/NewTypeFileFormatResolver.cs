using System;
using System.Globalization;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public interface IResolveNewTypeFileFormat
	{
		string ResolveFileFormat(string typeName, string indent);
	}
	
	internal delegate string FileFormatResolving(string typeName);
	
	public class NewTypeFileFormatResolver : IResolveNewTypeFileFormat
	{
		static FileFormatResolving interfaceFormat = name => {
			if (name.Length > 1 && name[0] == 'I' && char.GetUnicodeCategory(name[1]) == UnicodeCategory.UppercaseLetter)
				return "namespace {0} {{\r\n\tpublic interface {1}{{\r\n\t\t\r\n\t}}\r\n}}";
				return null;
		};
		
		FileFormatResolving[] resolvings = new []{interfaceFormat};
		
		public string ResolveFileFormat (string typeName, string indent)
		{
			if (typeName == null) throw new ArgumentNullException("typeName", "new type name cannot be null");
			string format = null;
			foreach (var resolvingAttempt in resolvings) {
				format = resolvingAttempt(typeName);
				if (format != null) return format;
			}
			
			return defaultClassFormat(typeName);
		}
		
		static FileFormatResolving defaultClassFormat = name => "namespace {0} {{\r\n\tpublic class {1}{{\r\n\t\t\r\n\t}}\r\n}}";
	}	
}

