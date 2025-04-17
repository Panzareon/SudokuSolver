using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public interface IConstraint
	{
		bool CanPlace(Board board, NextStep nextStep);
		void RemoveNotPossibleValues(Board board);
	}
}
