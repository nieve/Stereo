using MonoDevelop.Stereo.Refactoring.NewTypeFormatProviders;
using NUnit.Framework;
namespace MonoDevelop.Stereo.NewTypeFileContentProviderTest {
	[TestFixture]
	public class GetFormat
	{
		IProvideNewTypeFileContent resolver = new NewTypeFileContentProvider();
		readonly string eol = "\r\n";
		
		[Test]
		public void Returns_the_new_file_content ()
		{
			string fileFormat = resolver.GetFormat ("Something", "SomeNameSpace", eol);
			Assert.AreEqual("namespace SomeNameSpace {\r\nSomething\r\n}", fileFormat);
		}
	}
}