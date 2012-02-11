using System;

namespace MonoDevelop.Stereo.TextEditor
{
	public abstract class DuplicateText
	{
		protected string text;
		protected int offset;
		
		public DuplicateText (string text, int offset)
		{
			this.text = text;
			this.offset = offset;
		}

		public int Offset { get { return offset; } }
		
		public override string ToString () {
			return text;
		}
		
		public static implicit operator string(DuplicateText text){
			return text.ToString();
		}
	}
	
	public class EmptyDuplicateText : DuplicateText
	{
		public EmptyDuplicateText () : base(string.Empty, 0) {}
	}
	
	public class SelectedDuplicateText : DuplicateText
	{
		public SelectedDuplicateText (string selectedText, int offset) : base(selectedText, offset) {}
	}
	
	public class LineDuplicateText : DuplicateText
	{
		public LineDuplicateText (string line, int offset, string eol) : base(line + eol, offset) {}
	}
}

