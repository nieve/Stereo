using System;
using System.Globalization;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public interface IResolveNewTypeFileFormat
	{
		string ResolveFileFormat(string typeName);
	}
	
	public class NewTypeFileFormatResolver : IResolveNewTypeFileFormat
	{
		public string ResolveFileFormat (string typeName)
		{
			if (typeName.Length > 1 &&
			    typeName[0] == 'I' && char.GetUnicodeCategory(typeName[1]) == UnicodeCategory.UppercaseLetter) {
				return "namespace {0} {{\r\n\tpublic interface {1}{{\r\n\t\t\r\n\t}}\r\n}}";
			}
			
			return "namespace {0} {{\r\n\tpublic class {1}{{\r\n\t\t\r\n\t}}\r\n}}";
		}
	}
}

