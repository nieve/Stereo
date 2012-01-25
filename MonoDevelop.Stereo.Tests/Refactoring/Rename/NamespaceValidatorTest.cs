using MonoDevelop.Core;
using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.Rename;

namespace MonoDevelop.Stereo.Tests
{
	[TestFixture()]
	public class NamespaceValidatorTest
	{
		NamespaceValidator validator = new NamespaceValidator();
		
		[Test()]
		public void Empty_name_should_be_invalid ()
		{
			Assert.IsFalse(validator.ValidateName(null, null).IsValid);
			Assert.IsFalse(validator.ValidateName(null, string.Empty).IsValid);
		}
		
		[Test()]
		public void First_char_cannot_be_a_digit ()
		{
			Assert.IsFalse(validator.ValidateName(null, "3Test").IsValid);
		}
		
		[Test()]
		public void First_char_cannot_be_a_non_letter ()
		{
			Assert.IsFalse(validator.ValidateName(null, ")Test").IsValid);
		}
		
		[Test()]
		public void First_char_can_be_an_underscore_or_letter ()
		{
			Assert.IsTrue(validator.ValidateName(null, "_Test").IsValid);
			Assert.IsTrue(validator.ValidateName(null, "Test").IsValid);
		}
		
		[Test()]
		public void Last_char_cannot_be_a_dot ()
		{
			Assert.IsFalse(validator.ValidateName(null, "Test.").IsValid);
		}
		
		[Test()]
		public void Name_can_contain_a_dot ()
		{
			Assert.IsTrue(validator.ValidateName(null, "Test.Foo").IsValid);
		}
	}
}

