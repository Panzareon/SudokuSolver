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
		private TileSet(TileSetType tileSetType, int maxPositions)
		{
			this.TileSetType = tileSetType;
			this.MaxPositions = maxPositions;
		}

		public TileSet(TileSetType tileSetType, int maxPositions, Board board)
			: this(tileSetType, maxPositions)
		{
			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					this.PossiblePositions.AddLast(new Position(x, y));
				}
			}
		}

		public TileSetType TileSetType { get; }
		public int MaxPositions { get; }
		public LinkedList<Position> Positions { get; private init; } = new();

		public LinkedList<Position> PossiblePositions { get; private init; } = new();

		public void RemovePossiblePosition(Position position)
		{
			this.PossiblePositions.Remove(position);
		}

		public void RemovePossiblePosition(LinkedListNode<Position> position)
		{
			this.PossiblePositions.Remove(position);
		}

		public TileSet Clone()
		{
			return new TileSet(this.TileSetType, this.MaxPositions)
			{
				Positions = new(this.Positions),
				PossiblePositions = new(this.PossiblePositions),
			};
		}
	}
}
