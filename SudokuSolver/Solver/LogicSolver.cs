using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public class LogicSolver
	{
		private readonly IConstraint[] constraints;

		public LogicSolver(IConstraint[] constraints)
		{
			this.constraints = constraints;
		}
		public bool Solve(Board board)
		{
			this.RemoveNotPlaceable(board);
			foreach (var constraint in constraints)
			{
				if (!constraint.RemoveNotPossibleValues(board))
				{
					return false;
				}
			}

			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					var tile = board.GetTile(x, y);
					if (!tile.IsSet)
					{
						var possibleValues = board.GetPossibleValues(x, y);
						if (possibleValues.Values.Count == 1)
						{
							var newValue = possibleValues.Values.First();
							if (board.CanPlace(new NextStep(board) { SetX = x, SetY= y, NextValue = newValue }, constraints))
							{
								board.SetTile(x, y, new Tile { IsSet = true, Value = newValue });
							}
							else
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		private void RemoveNotPlaceable(Board board)
		{
			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					var possibleValues = board.GetPossibleValues(x, y);
					var tile = board.GetTile(x, y);
					var node = possibleValues.Values.First;
					while (node != null)
					{
						if ((tile.IsSet && node.Value == tile.Value)
							|| (!tile.IsSet && this.CanPlace(board, new NextStep(board) { SetX = x, SetY = y, NextValue = node.Value })))
						{
							node = node.Next;
						}
						else
						{
							var next = node.Next;
							possibleValues.Values.Remove(node);
							node = next;
						}
					}

					if (!tile.IsSet && possibleValues.Values.Count == 1)
					{
						board.SetTile(x, y, new Tile { IsSet = true, Value = possibleValues.Values.First() });
					}
				}
			}
		}

		private bool CanPlace(Board board, NextStep nextStep)
		{
			for (var i = 0; i < constraints.Length; i++)
			{
				if (!constraints[i].CanPlace(board, nextStep))
				{
					return false;
				}
			}

			return true;
		}
	}
}
