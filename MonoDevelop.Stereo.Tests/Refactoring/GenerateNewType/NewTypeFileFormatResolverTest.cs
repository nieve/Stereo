using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;

namespace MonoDevelop.Stereo.NewTypeFileFormatResolverTest
{
	[TestFixture()]
	public class ResolveFileFormat
	{
		IResolveNewTypeFileFormat resolver = new NewTypeFileFormatResolver();
		
		[Test()]
		public void Returns_interface_format_when_type_name_starts_with_I_and_another_capital_letter ()
		{
			string fileFormat = resolver.ResolveFileFormat ("IDoSomething", "");
			Assert.That(fileFormat.Contains("interface"), "interface format wasn't returned");
		}
		
		[Test()]
		public void Returns_class_format_when_type_name_is_not_interface ()
		{
			string fileFormat = resolver.ResolveFileFormat ("ImNotAnInterface", "");
			Assert.That(fileFormat.Contains("class"), "class format wasn't returned");
		}
	}
}

