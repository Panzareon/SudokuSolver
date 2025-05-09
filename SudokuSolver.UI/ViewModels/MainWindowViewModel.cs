using ReactiveUI;
using SudokuSolver.Constraints;
using SudokuSolver.Solver;
using System.Collections.ObjectModel;

namespace SudokuSolver.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private BoardViewModel board = new();

		public BoardViewModel Board
		{
			get => board;
			set => this.RaiseAndSetIfChanged(ref board, value);
		}
		public ObservableCollection<TileViewModel> Tiles { get; } = new();
		public string Rows => "* * * *";
	}
}
