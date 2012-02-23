using MonoDevelop.Stereo.Refactoring.NewTypeFormatProviders;
using NUnit.Framework;

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
}