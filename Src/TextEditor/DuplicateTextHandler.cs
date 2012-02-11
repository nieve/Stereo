using System;
using MonoDevelop.Components.Commands;

namespace MonoDevelop.Stereo.TextEditor
{
	public class DuplicateTextHandler : CommandHandler
	{
		ITextDuplicationContext context;
		
		public DuplicateTextHandler (ITextDuplicationContext ctx)
		{
			context = ctx;
		}
		
		public DuplicateTextHandler ()
		{
			context = new TextDuplicationContext();
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = context.ActiveDocumentAndEditorExist();
		}
		
		protected override void Run ()
		{
	      	var textToDuplicate = context.GetTextToDuplicate();
			context.AppendDuplicatedText(textToDuplicate);
		}
	}
}

