using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;

namespace MonoDevelop.Stereo.NewTypeFileFormatResolverTest
{
	[TestFixture]
	public class ResolveFileFormat
	{
		IResolveNewTypeFormat resolver = new NewTypeFormatResolver();
		readonly string eol = "\r\n";
		
		//TODO: replace with ResolveNewFile tests.
//		[Test]
//		public void Returns_the_exact_new_interface_file_format ()
//		{
//			string fileFormat = resolver.ResolveFileFormat ("IDoSomething", "\t", eol, true);
//			Assert.AreEqual("namespace {0} {{\r\n\tpublic interface {1} {{\r\n\t\t\r\n\t}}\r\n}}", fileFormat);
//		}
		
//		[Test]
//		public void Returns_the_exact_new_class_file_format ()
//		{
//			string fileFormat = resolver.ResolveFileFormat ("ImNotAnInterface", "\t", eol, true);
//			Assert.AreEqual("namespace {0} {{\r\n\tpublic class {1} {{\r\n\t\t\r\n\t}}\r\n}}", fileFormat);
//		}
		
		[Test]
		public void Returns_the_exact_interface_format ()
		{
			string fileFormat = resolver.ResolveFileFormat ("IDoSomething", "", eol);
			Assert.AreEqual("public interface {1} {{\r\n\t\r\n}}", fileFormat);
		}
		
		[Test]
		public void Returns_the_exact_class_format ()
		{
			string fileFormat = resolver.ResolveFileFormat ("ImNotAnInterface", "", eol);
			Assert.AreEqual("public class {1} {{\r\n\t\r\n}}", fileFormat);
		}
	}
}

