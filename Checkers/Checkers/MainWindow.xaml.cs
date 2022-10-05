using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Checkers.Models;

namespace Checkers;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static readonly Game CurrentGame = new();

    public MainWindow()
    {
        InitializeComponent();

        CreateTable();
        AddCheckers();

        Execute(CurrentGame.DrawPossiblePickCommand());
    }

    private static Tuple<char, int> ParseButtonName(string name)
    {
        if (name == "UnDo")
            return new Tuple<char, int>('u', 0);

        var column = name.First();
        var row = name.Last() - '0';

        return new Tuple<char, int>(column, row);
    } // to visual coords

    private void ButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button pressedButton)
        {
            MessageBox.Show("Нажатие на объект, не являющийся кнопкой");
            return;
        }

        var cellColumn = ParseButtonName(pressedButton.Name).Item1;
        var cellRow = ParseButtonName(pressedButton.Name).Item2;

        var todo = CurrentGame.Turn(cellColumn, cellRow);

        foreach (var line in todo) Execute(line);
    }

    private void CreateTable()
    {
        for (var rowIterator = 0; rowIterator < 8; rowIterator++)
        for (var columnIterator = 1; columnIterator < 9; columnIterator++)
            if (rowIterator % 2 == (columnIterator - 1) % 2)
            {
                var whiteBorder = new Border
                {
                    Background = Brushes.White
                };

                Grid.SetColumn(whiteBorder, columnIterator);
                Grid.SetRow(whiteBorder, rowIterator);

                GridBoard.Children.Add(whiteBorder);
            }
            else
            {
                var c = (char) ('a' + columnIterator - 1);
                var r = 8 - rowIterator;

                var blackButton = new Button
                {
                    Name = $"{c}" + $"{r}",
                    Background = Brushes.Black
                };

                blackButton.Click += ButtonClick;

                Grid.SetColumn(blackButton, columnIterator);
                Grid.SetRow(blackButton, rowIterator);

                GridBoard.Children.Add(blackButton);
            }
    }

    private void AddCheckers()
    {
        for (var rowIterator = 0; rowIterator < 8; rowIterator++)
        for (var columnIterator = 1; columnIterator < 9; columnIterator++)
            if (rowIterator % 2 != (columnIterator - 1) % 2 && rowIterator is < 3 or > 4)
            {
                var checker = new Ellipse
                {
                    Fill = rowIterator < 3 ? Brushes.Goldenrod : Brushes.Silver,
                    Width = 40,
                    Height = 40,
                    IsHitTestVisible = false,
                    Name = $"{(char) (columnIterator - 1 + 'a')}{8 - rowIterator}"
                };

                Grid.SetColumn(checker, columnIterator);
                Grid.SetRow(checker, rowIterator);

                GridBoard.Children.Add(checker);
            }
    }

    private void Execute(string command)
    {
        //MessageBox.Show(command);

        if (command.Contains("message:")) MessageBox.Show(command.Remove(0, 9));

        if (command.Contains("select_figure:"))
        {
            var name = command.Split(" ").Last();

            var selectedFigure = new Ellipse
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 3,
                Width = 40,
                Height = 40,
                IsHitTestVisible = false,
                Tag = "selector"
            };

            Grid.SetColumn(selectedFigure, name.First() - 'a' + 1);
            Grid.SetRow(selectedFigure, '8' - name.Last());

            GridBoard.Children.Add(selectedFigure);
        }

        if (command.Contains("unselect"))
            for (var i = GridBoard.Children.Count - 1; i >= 0; --i)
                if (GridBoard.Children[i] is Ellipse cellMarker && (string) cellMarker.Tag == "selector")
                {
                    GridBoard.Children.RemoveAt(i);
                    break;
                }

        if (command.Contains("mark_cells:"))
        {
            if (command.Length <= 12) return;

            var line = command.Remove(0, 12).Split(" ");

            if (line.Length == 0) return;

            foreach (var name in line)
            {
                var selectedFigure = new Rectangle
                {
                    Stroke = Brushes.Aqua,
                    StrokeThickness = 2,
                    Width = 50,
                    Height = 50,
                    IsHitTestVisible = false,
                    Tag = "marker"
                };

                Grid.SetColumn(selectedFigure, name.First() - 'a' + 1);
                Grid.SetRow(selectedFigure, '8' - name.Last());

                GridBoard.Children.Add(selectedFigure);
            }
        }

        if (command.Contains("unmark_cells"))
            for (var i = GridBoard.Children.Count - 1; i >= 0; --i)
                if (GridBoard.Children[i] is Rectangle cellMarker && (string) cellMarker.Tag == "marker")
                    GridBoard.Children.RemoveAt(i);

        if (command.Contains("move:"))
        {
            var line = command.Split(" ");
            var nameFrom = line[1];
            var nameTo = line[2];
            var color = line[3];
            var status = line[4];

            Execute($"draw: {nameTo} {color} {status}");
            Execute($"erase: {nameFrom}");
            Execute("unselect");
            Execute($"select_figure: {nameTo}");
        }

        if (command.Contains("erase:"))
        {
            var line = command.Split(" ");
            var name = line[1];

            for (var i = GridBoard.Children.Count - 1; i >= 0; --i)
                if (GridBoard.Children[i] is Ellipse figure && figure.Name == name)
                {
                    GridBoard.Children.RemoveAt(i);
                    break;
                }
        }

        if (command.Contains("transform:"))
        {
            var line = command.Split(" ");
            var name = line[1];
            var color = line[2];

            for (var i = GridBoard.Children.Count - 1; i >= 0; --i)
                if (GridBoard.Children[i] is Ellipse figure && figure.Name == name)
                {
                    (GridBoard.Children[i] as Ellipse)!.Fill = color == "White" ? Brushes.Teal : Brushes.Olive;
                    break;
                }
        }

        if (command.Contains("draw:"))
        {
            var line = command.Split(" ");
            var name = line[1];
            var color = line[2];
            var status = line[3];

            Brush fill;
            if (color == "White")
            {
                if (status == "Checker")
                    fill = Brushes.Silver;
                else
                    fill = Brushes.Teal;
            }
            else
            {
                if (status == "Checker")
                    fill = Brushes.Goldenrod;
                else
                    fill = Brushes.Olive;
            }

            var checker = new Ellipse
            {
                Fill = fill,
                Width = 40,
                Height = 40,
                IsHitTestVisible = false,
                Name = $"{name.First()}{name.Last()}"
            };
            Grid.SetColumn(checker, name.First() - 'a' + 1);
            Grid.SetRow(checker, '8' - name.Last());
            GridBoard.Children.Add(checker);
        }

        if (command.Contains("transformBack:"))
        {
            var line = command.Split(" ");
            var name = line[1];
            var color = line[2];

            for (var i = GridBoard.Children.Count - 1; i >= 0; --i)
                if (GridBoard.Children[i] is Ellipse figure && figure.Name == name)
                {
                    (GridBoard.Children[i] as Ellipse)!.Fill = color == "Teal" ? Brushes.Silver : Brushes.Goldenrod;
                    break;
                }
        }

        if (command.Contains("log:"))
        {
            var line = command.Split(" ");
            var nameFrom = line[1];
            var nameTo = line[2];
            var color = line[3];
            var status = line[4];

            Brush fill;
            if (color == "White")
            {
                if (status == "Checker")
                    fill = Brushes.Silver;
                else
                    fill = Brushes.Teal;
            }
            else
            {
                if (status == "Checker")
                    fill = Brushes.Goldenrod;
                else
                    fill = Brushes.Olive;
            }

            var printTurn = new TextBlock
            {
                Text = $"{nameFrom} - {nameTo}",
                FontSize = 20,
                Foreground = fill,
                TextAlignment = TextAlignment.Center
            };
            TurnLog.Children.Add(printTurn);
        }

        if (command.Contains("delete_last_log")) TurnLog.Children.RemoveAt(TurnLog.Children.Count - 1);
    }
}