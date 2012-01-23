using System;

namespace MonoDevelop.Stereo
{
	internal static class StringsExtensions
	{
		public static string ToFormat(this string str, params object[] args){
			return string.Format(str, args);
		}
	}
}

