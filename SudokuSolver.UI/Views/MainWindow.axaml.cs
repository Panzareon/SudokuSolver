using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using SudokuSolver.UI.ViewModels;
using System;
using System.Threading.Tasks;

namespace SudokuSolver.UI.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void NewSudokuClick(object sender, RoutedEventArgs args)
		{
			var newSudokuDialog = new NewSudokuDialog();
			await newSudokuDialog.ShowDialog(this);
			if (newSudokuDialog.Ok && this.DataContext is MainWindowViewModel mainWindowViewModel)
			{
				mainWindowViewModel.Board = new BoardViewModel(new Model.Board(newSudokuDialog.WidthValue, newSudokuDialog.HeightValue));
			}
		}
	}
}