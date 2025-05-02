using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public interface IChaosConstructionConstraint : IConstraint
	{
		void InitializeBoard(Board board);

		bool CanSet(Board board, TileSet tileSet, Position position);
	}
}
