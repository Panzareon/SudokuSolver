using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public record PositionDelta(int deltaX, int deltaY)
	{
		public static Position operator+ (Position left, PositionDelta right)
		{
			return new Position(left.X + right.deltaX, left.Y + right.deltaY);
		}
	}
}
