using System;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Core;

namespace Stereo
{
	public class NamespaceValidator : INameValidator
	{
		public NamespaceValidator ()
		{
		}

		public ValidationResult ValidateName (INode visitable, string name)
		{
			if (string.IsNullOrEmpty(name))
				return ValidationResult.CreateError(GettextCatalog.GetString("Name must not be empty."));
			char c1 = name[0];
			if (!char.IsLetter(c1) && (int) c1 != 95)
				return ValidationResult.CreateError(GettextCatalog.GetString("Name must start with a letter or '_'"));
			char lc = name[name.Length - 1];
			if (!char.IsLetterOrDigit(lc) && (int) lc != 95)
				return ValidationResult.CreateError("Name can only end with a letter, digit and '_'");
			for (int index = 1; index < name.Length - 1; ++index) {
				char c2 = name[index];
				if (!char.IsLetterOrDigit(c2) && c2 != '.' && (int) c2 != 95)
					return ValidationResult.CreateError("Name can only contain letters, digits, dots and '_'");
			}
			return ValidationResult.Valid;
		}
	}
}

