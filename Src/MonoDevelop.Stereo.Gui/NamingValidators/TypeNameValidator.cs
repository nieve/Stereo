using System;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Core;

namespace MonoDevelop.Stereo.Gui
{
	public class TypeNameValidator : INameValidator
	{
		public TypeNameValidator ()
		{
		}

		public MonoDevelop.Projects.CodeGeneration.ValidationResult ValidateName (MonoDevelop.Projects.Dom.INode visitable, string name)
		{
			if (string.IsNullOrEmpty(name.Trim()))
				return ValidationResult.CreateError(GettextCatalog.GetString("Name must not be empty."));
			
			char c1 = name[0];
			if (!char.IsLetter(c1))
				return ValidationResult.CreateError(GettextCatalog.GetString("Name must start with a letter"));
			
			for (int index = 1; index < name.Length - 1; ++index) {
				char c2 = name[index];
				if (!char.IsLetterOrDigit(c2))
					return ValidationResult.CreateError("Name can only contain letters or digits");
			}
			return ValidationResult.Valid;
		}
	}
}

