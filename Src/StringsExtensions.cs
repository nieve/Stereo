using System;

namespace MonoDevelop.Stereo
{
	public static class StringsExtensions
	{
		public static string ToFormat(this string str, params object[] args){
			return string.Format(str, args);
		}
	}
}

