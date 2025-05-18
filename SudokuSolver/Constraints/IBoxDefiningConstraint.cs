using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public interface IBoxDefiningConstraint : IConstraint
	{
		void InitializeBoxPositions(Board board);
	}
}
