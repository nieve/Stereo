using System;
using System.Globalization;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public interface IResolveNewTypeFileFormat
	{
		string ResolveFileFormat(string typeName);
	}
	
	internal delegate string FileFormatResolving(string typeName);
	
	public class NewTypeFileFormatResolver : IResolveNewTypeFileFormat
	{
		static FileFormatResolving interfaceFormat = name => {
			if (name.Length > 1 && name[0] == 'I' && char.GetUnicodeCategory(name[1]) == UnicodeCategory.UppercaseLetter)
				return "namespace {0} {{\r\n\tpublic interface {1}{{\r\n\t\t\r\n\t}}\r\n}}";
				return null;
		};
		
		static FileFormatResolving classFormat = name => "namespace {0} {{\r\n\tpublic class {1}{{\r\n\t\t\r\n\t}}\r\n}}";
		
		FileFormatResolving[] resolvings = new []{interfaceFormat, classFormat};
		
		public string ResolveFileFormat (string typeName)
		{
			if (typeName == null) throw new ArgumentNullException("typeName", "new type name cannot be null");
			string format = null;
			foreach (var resolvingAttempt in resolvings) {
				format = resolvingAttempt(typeName);
				if (format != null) return format;
			}
			
			throw new ArgumentException("typeName", "{0} cannot be used as a new type name".ToFormat(typeName));
		}
	}	
}

