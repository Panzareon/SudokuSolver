using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SudokuSolver.Constraints;
using SudokuSolver.Solver;
using System;

namespace SudokuSolver.UI.Views;

public partial class NewSudokuDialog : Window
{
	public NewSudokuDialog()
	{
		InitializeComponent();
	}

	public bool Ok { get; private set; }

	public int WidthValue => (int?)this.WidthInput.Value ?? throw new InvalidOperationException("Value of width is not set");

	public int HeightValue => (int?)this.HeightInput.Value ?? throw new InvalidOperationException("Value of height is not set");

	public IConstraint[] CreateConstraints()
	{
		return [..ConstraintFactory.DefaultSudoku(
			(int?)this.BoxWidth.Value ?? throw new InvalidOperationException("Value of box-width is not set"),
			(int?)this.BoxHeight.Value ?? throw new InvalidOperationException("Value of box-height is not set"))];
	}

	private void OkClick(object sender, RoutedEventArgs args)
	{
		this.Ok = true;
		this.Close();
	}

	private void CancelClick(object sender, RoutedEventArgs args)
	{
		this.Ok = false;
		this.Close();
	}
}