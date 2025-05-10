using ReactiveUI;
using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.UI.ViewModels
{
	public class TileViewModel : ViewModelBase
	{
		private readonly Board board;

		public TileViewModel(Board board, int x, int y, IEnumerable<ConstraintViewModel> constraints)
		{
			this.board = board;
			this.X = x;
			this.Y = y;
			this.Constraints = [.. constraints];
		}

		public string Display => this.board.GetTile(this.X, this.Y).Display;

		public bool ShowPossibleValues => !this.board.GetTile(this.X, this.Y).IsSet;

		public string PossibleValues
		{
			get
			{
				var values = this.board.GetPossibleValues(this.X, this.Y).Values;
				var valuesPerLine = MathF.Ceiling(MathF.Sqrt(values.Count));
				var displayValues = new StringBuilder();
				var taken = 0;
				foreach (var value in values)
				{
					if (taken >= valuesPerLine)
					{
						taken = 0;
						displayValues.AppendLine();
					}

					displayValues.Append(value.ToString());
					taken++;
				}
				return displayValues.ToString();
			}
		}

		public int X { get; }

		public int Y { get; }

		public ObservableCollection<ConstraintViewModel> Constraints { get; }

		public void Refresh()
		{
			this.RaisePropertyChanged(nameof(Display));
			this.RaisePropertyChanged(nameof(ShowPossibleValues));
			this.RaisePropertyChanged(nameof(PossibleValues));
		}

		public void SetTile(int pressedNumber)
		{
			this.board.SetTile(this.X, this.Y, new Tile { IsSet = true, Value = pressedNumber });
			this.Refresh();
		}
	}
}
