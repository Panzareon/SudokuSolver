using ReactiveUI;
using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.UI.ViewModels
{
	public class TileViewModel : ViewModelBase
	{
		private readonly Board board;

		public TileViewModel(Board board, int x, int y)
		{
			this.board = board;
			this.X = x;
			this.Y = y;
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

		public void Refresh()
		{
			this.RaisePropertyChanged(nameof(Display));
			this.RaisePropertyChanged(nameof(ShowPossibleValues));
			this.RaisePropertyChanged(nameof(PossibleValues));
		}
	}
}
