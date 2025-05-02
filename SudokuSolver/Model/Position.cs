using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	public record Position(int X, int Y)
	{
		public bool IsAdjacent(Position other)
		{
			return ((other.X == this.X + 1 || other.X == this.X - 1) && other.Y == this.Y)
								|| ((other.Y == this.Y + 1 || other.Y == this.Y - 1) && other.X == this.X);
		}
	}
}
