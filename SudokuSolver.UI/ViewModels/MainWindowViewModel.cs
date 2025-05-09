using SudokuSolver.Constraints;
using SudokuSolver.Solver;
using System.Collections.ObjectModel;

namespace SudokuSolver.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public BoardViewModel Board { get; set; } = new();
		public ObservableCollection<TileViewModel> Tiles { get; } = new();
		public string Rows => "* * * *";
	}
}
