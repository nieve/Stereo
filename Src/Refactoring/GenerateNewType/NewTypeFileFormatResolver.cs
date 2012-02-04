using System;
using System.Globalization;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public interface IResolveNewTypeFileFormat
	{
		string ResolveFileFormat(string typeName, string indent, string eol, bool isNewFile);
	}
	
	internal delegate string FileFormatResolving(string typeName, string indent, string eol, bool isNewFile);
	
	public class NewTypeFileFormatResolver : IResolveNewTypeFileFormat
	{
		static string fileFormat= @"namespace {{0}} {{{{{0}}}}}";
		static string interfaceFileFormat = "<indent>public interface {1}{{<eol><indent>\t<eol><indent>}}";
		static string classFormat = "<indent>public class {1}{{<eol><indent>\t<eol><indent>}}";
		
		static FileFormatResolving interfaceFormat = (name, indent, eol, isNewFile) => {
			var ifrmt = interfaceFileFormat.Replace("<indent>", indent);
			ifrmt = ifrmt.Replace("<eol>", eol);
			if (name.Length > 1 && name[0] == 'I' && char.GetUnicodeCategory(name[1]) == UnicodeCategory.UppercaseLetter)
				return isNewFile ? fileFormat.ToFormat(eol + ifrmt + eol) : ifrmt;
			return null;
		};
		
		FileFormatResolving[] resolvings = new []{interfaceFormat};
		
		public string ResolveFileFormat (string typeName, string indent, string eol, bool isNewFile)
		{
			if (typeName == null) throw new ArgumentNullException("typeName", "new type name cannot be null");
			string format = null;
			foreach (var resolvingAttempt in resolvings) {
				format = resolvingAttempt(typeName, indent, eol, isNewFile);
				if (format != null) return format;
			}
			
			return defaultClassFormat(typeName, indent, eol, isNewFile);
		}
		
		static FileFormatResolving defaultClassFormat = (name, indent, eol, isNewFile) => {
			var clsfrmt = classFormat.Replace("<indent>", indent);
			clsfrmt = clsfrmt.Replace("<eol>", eol);
			return isNewFile ? fileFormat.ToFormat(eol + clsfrmt + eol) : clsfrmt;
		};
	}	
}

