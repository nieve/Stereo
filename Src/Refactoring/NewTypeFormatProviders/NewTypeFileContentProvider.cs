using System;

namespace MonoDevelop.Stereo.Refactoring.NewTypeFormatProviders
{
	public interface IProvideNewTypeFileContent
	{
		string GetFormat(string typeContent, string nspace, string eol);
	}
	
	public class NewTypeFileContentProvider : IProvideNewTypeFileContent
	{
		static string fileFormat= @"namespace {{0}} {{{{{0}}}}}";
		
		public string GetFormat (string typeContent, string nspace, string eol)
		{
			typeContent = eol + typeContent + eol;
			typeContent = typeContent.Replace("{", "{{").Replace("}", "}}");
			var preNspace = fileFormat.ToFormat(typeContent);
			return preNspace.ToFormat (nspace);
		}
	}
}

