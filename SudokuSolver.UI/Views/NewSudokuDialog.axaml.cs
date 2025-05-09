using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace SudokuSolver.UI.Views;

public partial class NewSudokuDialog : Window
{
	public NewSudokuDialog()
	{
		InitializeComponent();
	}

	public bool Ok { get; private set; }

	public int WidthValue => (int?)this.Width.Value ?? throw new InvalidOperationException("Value of width is not set");

	public int HeightValue => (int?)this.Height.Value ?? throw new InvalidOperationException("Value of height is not set");

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