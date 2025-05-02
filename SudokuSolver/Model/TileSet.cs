using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	public enum TileSetType
	{
		/// <summary>
		/// The tile set represents an additional sudoku region.
		/// </summary>
		SudokuRegion,
	}

	public class TileSet
	{
		public TileSet(TileSetType tileSetType, int maxPositions)
		{
			this.TileSetType = tileSetType;
			this.MaxPositions = maxPositions;
		}

		public TileSetType TileSetType { get; }
		public int MaxPositions { get; }
		public LinkedList<Position> Positions { get; init; } = new();

		public TileSet Clone()
		{
			return new TileSet(this.TileSetType, this.MaxPositions)
			{
				Positions = new(this.Positions)
			};
		}
	}
}
