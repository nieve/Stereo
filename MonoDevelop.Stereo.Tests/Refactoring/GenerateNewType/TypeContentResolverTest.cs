using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;

namespace MonoDevelop.Stereo.TypeContentResolverTest
{
	[TestFixture]
	public class GetNewTypeContent
	{
		IResolveTypeContent resolver = new TypeContentResolver();
		readonly string eol = "\r\n";
		
		[Test]
		public void Returns_the_exact_interface_format ()
		{
			string fileFormat = resolver.GetNewTypeContent ("IDoSomething", "", eol);
			Assert.AreEqual("public interface IDoSomething {\r\n\t\r\n}", fileFormat);
		}
		
		[Test]
		public void Returns_the_exact_class_format ()
		{
			string fileFormat = resolver.GetNewTypeContent ("ImNotAnInterface", "", eol);
			Assert.AreEqual("public class ImNotAnInterface {\r\n\t\r\n}", fileFormat);
		}
	}
	
	[TestFixture]
	public class GetNewTypeFileContent
	{
		IResolveTypeContent resolver = new TypeContentResolver();
		readonly string eol = "\r\n";
		
		[Test]
		public void Returns_the_new_file_content ()
		{
			string fileFormat = resolver.GetNewTypeFileContent ("Something", "SomeNameSpace", eol);
			Assert.AreEqual("namespace SomeNameSpace {\r\nSomething\r\n}", fileFormat);
		}
	}
}

