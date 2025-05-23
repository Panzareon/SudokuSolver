﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	public class Board
	{
		private readonly Tile[] tiles;
		private readonly PossibleValues[] possibleValues;
		public Board(int width, int height, int? maxNumberOverride = null)
		{
			this.Width = width;
			this.Height = height;
			this.MaxNumber = maxNumberOverride ?? (width > height ? width : height);
			this.tiles = new Tile[width * height];
			this.possibleValues = new PossibleValues[width * height];
			this.ResetPossibleValues();

			this.TileSets = new TileSets(this);
		}

		public int Width { get; }
		public int Height { get; }

		public int MinNumber { get; } = 1;

		public int MaxNumber { get; }

		public TileSets TileSets { get; private init; }

		public bool IsInitialized { get; set; }

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

		public PossibleValues GetPossibleValues(int x, int y)
		{
			return this.possibleValues[x + y * this.Width];
		}

		public void ResetPossibleValues()
		{
			for (var i = 0; i < this.possibleValues.Length; i++)
			{
				this.possibleValues[i] = new PossibleValues(this.MaxNumber, i);
			}
		}

		public Board Clone()
		{
			var clone = new Board(this.Width, this.Height, this.MaxNumber)
			{
				TileSets = new TileSets(this.TileSets),
				IsInitialized = this.IsInitialized,
			};
			this.tiles.CopyTo(clone.tiles, 0);
			for (var i = 0; i < this.possibleValues.Length; i++)
			{
				var existingNode = this.possibleValues[i].Values.First;
				var copyList = clone.possibleValues[i].Values;
				var copyNode = copyList.First;
				while (copyNode != null)
				{
					if (copyNode.Value != existingNode?.Value)
					{
						var nextNode = copyNode.Next;
						copyList.Remove(copyNode);
						copyNode = nextNode;
					}
					else
					{
						copyNode = copyNode.Next;
						existingNode = existingNode.Next;
					}
				}
			}

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
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			var result = new StringBuilder();
			for (int y = 0; y < this.Height; y++)
			{
				for (int x = 0; x < this.Width; x++)
				{
					result.Append(this.GetTile(x, y).Display);
				}
				result.AppendLine();
			}

			return result.ToString();
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
