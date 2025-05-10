using Avalonia;
using SudokuSolver.Constraints;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.UI.ViewModels
{
	public class ConstraintViewModel : ViewModelBase
	{
		private readonly IConstraint constraint;

		public ConstraintViewModel(DefaultSudokuBox constraint, int x, int y)
		{
			const int Thickness = 2;
			this.constraint = constraint;
			this.BorderThickness = new Thickness(
				x % constraint.BoxWidth == 0 ? Thickness : 0,
				y % constraint.BoxHeight == 0 ? Thickness : 0,
				x % constraint.BoxWidth == constraint.BoxWidth - 1 ? Thickness : 0,
				y % constraint.BoxHeight == constraint.BoxHeight - 1 ? Thickness : 0);
		}

		public Thickness BorderThickness { get; } = new Thickness(0, 0, 0, 0);
	}
}
