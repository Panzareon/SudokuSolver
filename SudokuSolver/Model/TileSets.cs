using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	public class TileSets
	{
		private readonly int[] cachedPositions;
		public List<TileSet> Sets { get; } = [];

		private int width;

		public TileSets(Board board)
		{
			this.width = board.Width;
			this.cachedPositions = [.. Enumerable.Repeat(-1, board.Width * board.Height)];
			this.Sets = new List<TileSet>();
		}

		public TileSets(TileSets source)
		{
			this.width = source.width;
			this.cachedPositions = new int[source.cachedPositions.Length];
			source.cachedPositions.CopyTo(this.cachedPositions, 0);
			for (var i = 0; i < source.Sets.Count; i++)
			{
				this.Sets.Add(source.Sets[i].Clone());
			}
		}

		public TileSet? GetTileSetFromPosition(Position position)
		{
			var index = this.cachedPositions[position.X + position.Y * this.width];
			return index < 0 ? null : this.Sets[index];
		}
		public void AddPosition(TileSet tileSet, Position position)
		{
			this.AddPosition(this.Sets.IndexOf(tileSet), position);
		}

		public void AddPosition(int tileSetIndex, Position position)
		{
			var tileSet = this.Sets[tileSetIndex];
			Debug.Assert(!tileSet.Positions.Contains(position), "Positions shouldn't be added twice");
			tileSet.Positions.AddLast(position);
			this.cachedPositions[position.X + position.Y * this.width] = tileSetIndex;
			if (tileSet.Positions.Count == tileSet.MaxPositions)
			{
				var current = tileSet.PossiblePositions.First;
				while (current != null)
				{
					if (tileSet.Positions.Contains(current.Value))
					{
						current = current.Next;
					}
					else
					{
						var next = current.Next;
						tileSet.PossiblePositions.Remove(current);
						current = next;
					}
				}
			}
		}

	}
}
