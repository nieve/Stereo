using System;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Refactoring.QuickFixes;

namespace MonoDevelop.Stereo.Refactoring.CreateDerivedType
{
	public class CreateDerivedTypeRefactoring : RefactoringOperation, IRefactorTask, ICreateDerivedTypeRefactoring
	{
		INonConcreteTypeContext context;
		
		public CreateDerivedTypeRefactoring ()
		{
			context = new NonConcreteTypeContext();
		}		
		
		public CreateDerivedTypeRefactoring (INonConcreteTypeContext context)
		{
			this.context = context;
		}

		public override void Run (RefactoringOptions options)
		{
			
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			return IsValid ();
		}
		
		public bool IsValid ()
		{
			return context.IsCurrentLocationNonConcreteType();
		}
		
		public string Title  { get {return "Create derived type";}}
		public int Position { get {return 2;}}
	}
	
	public interface ICreateDerivedTypeRefactoring
	{
		bool IsValid();
		void Run(RefactoringOptions options);
	}
}

