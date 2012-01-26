using MonoDevelop.Core;
using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.Rename;

namespace MonoDevelop.Stereo.NamespaceValidatorTest
{
	[TestFixture()]
	public class ValidateName
	{
		NamespaceValidator validator = new NamespaceValidator();
		
		[Test()]
		public void Invalidates_empty_name ()
		{
			Assert.IsFalse(validator.ValidateName(null, null).IsValid);
			Assert.IsFalse(validator.ValidateName(null, string.Empty).IsValid);
		}
		
		[Test()]
		public void Invalidates_first_char_digit ()
		{
			Assert.IsFalse(validator.ValidateName(null, "3Test").IsValid);
		}
		
		[Test()]
		public void Invalidates_non_letter_first_char ()
		{
			Assert.IsFalse(validator.ValidateName(null, ")Test").IsValid);
		}
		
		[Test()]
		public void Validates_first_char_underscore_or_letter ()
		{
			Assert.IsTrue(validator.ValidateName(null, "_Test").IsValid);
			Assert.IsTrue(validator.ValidateName(null, "Test").IsValid);
		}
		
		[Test()]
		public void Invalidates_last_char_dot ()
		{
			Assert.IsFalse(validator.ValidateName(null, "Test.").IsValid);
		}
		
		[Test()]
		public void Validates_Name_containing_a_dot ()
		{
			Assert.IsTrue(validator.ValidateName(null, "Test.Foo").IsValid);
		}
	}
}

