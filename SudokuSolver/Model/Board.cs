using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	public class Board
	{
		private Tile[] tiles;
		public Board(int width, int height)
		{
			this.Width = width;
			this.Height = height;
			this.tiles = new Tile[width * height];
		}

		public int Width { get; }
		public int Height { get; }

		public int MaxNumber { get; } = 9;
		public int BoxSize { get; } = 3;

		public Tile GetTile(int x, int y)
		{
			return this.tiles[x + y * this.Width];
		}


		public Tile GetTile(Position position)
		{
			return this.GetTile(position.X, position.Y);
		}

		public Tile SetTile(int x, int y, Tile tile)
		{
			return this.tiles[x + y * this.Width] = tile;
		}

		public Board Clone()
		{
			var clone = new Board(this.Width, this.Height);
			this.tiles.CopyTo(clone.tiles, 0);
			return clone;
		}

		public override bool Equals(object? obj)
		{
			if (base.Equals(obj)) return true;
			if ( obj is not Board board)
			{
				return false;
			}

			for (var i = 0; i < this.tiles.Length; i++)
			{
				if (!this.tiles[i].Equals(board.tiles[i]))
				{
					return false;
				}
			}

			return true;
		}
		public override int GetHashCode()
		{
			var result = 0;
			for (var i = 0; i < this.tiles.Length; i++)
			{
				result ^= this.tiles[i].GetHashCode();
			}

			return result;
		}

		public void WriteToConsole()
		{
			for (int y = 0; y < this.Height; y++)
			{
				for (int x = 0; x < this.Width; x++)
				{
					Console.Write(this.GetTile(x, y).Display);
				}
				Console.WriteLine();
			}
		}

		public static Board Parse(string boardRepresentation)
		{
			var lines = boardRepresentation.ReplaceLineEndings().Split(Environment.NewLine).ToList();
			var board = new Board(lines[0].Length, lines.Count);
			for (var y = 0; y < lines.Count; y++)
			{
				var line = lines[y];
				for (var x = 0; x < line.Length; x++)
				{
					if (int.TryParse(line[x].ToString(), out var value))
					{
						board.SetTile(x, y, new Tile { IsSet = true, Value = value });
					}
				}
			}

			return board;
		}
	}
}
