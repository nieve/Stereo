using System;
using MonoDevelop.Components.Commands;

namespace MonoDevelop.Stereo.TextEditor
{
	public class DuplicateTextHandler : CommandHandler
	{
		IParseDocument docParser;
		
		public DuplicateTextHandler (IParseDocument parser)
		{
			docParser = parser;
		}
		
		public DuplicateTextHandler ()
		{
			docParser = new DocumentParser();
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = docParser.ActiveDocumentAndEditorExist();
		}
		
		protected override void Run ()
		{
	      	var textToDuplicate = docParser.GetTextToDuplicate();
			docParser.AppendDuplicatedText(textToDuplicate);
		}
	}
}

