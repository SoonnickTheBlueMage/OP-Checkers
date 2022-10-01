using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Checkers.Models;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

            foreach (var line in todo)
            {
                Execute(line);
            }
        }

        private void CreateTable()
        {
            for (var rowIterator = 0; rowIterator < 8; rowIterator++)
            {
                for (var columnIterator = 1; columnIterator < 9; columnIterator++)
                {
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

                        var blackButton = new Button()
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
            }
        }

        private void AddCheckers()
        {
            for (var rowIterator = 0; rowIterator < 8; rowIterator++)
            {
                for (var columnIterator = 1; columnIterator < 9; columnIterator++)
                {
                    if ((rowIterator % 2 != (columnIterator - 1) % 2) && rowIterator is < 3 or > 4)
                    {
                        var checker = new Ellipse()
                        {
                            Fill = (rowIterator < 3) ? Brushes.Goldenrod : Brushes.Silver,
                            Width = 40,
                            Height = 40,
                            IsHitTestVisible = false,
                            Name = $"{(char)(columnIterator - 1 + 'a')}{8 - rowIterator}"
                        };

                        Grid.SetColumn(checker, columnIterator);
                        Grid.SetRow(checker, rowIterator);

                        GridBoard.Children.Add(checker);
                    }
                }
            }
        }

        private void Execute(string command)
        {
            MessageBox.Show(command);
            if (command.Contains("message:"))
            {
                MessageBox.Show(command.Remove(0, 9));
            }

            if (command.Contains("select_figure:"))
            {
                var name = command.Split(" ").Last();

                var selectedFigure = new Ellipse()
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
            {
                for (int i = GridBoard.Children.Count - 1; i >= 0; --i)
                {
                    if (GridBoard.Children[i] is Ellipse cellMarker && (string) cellMarker.Tag == "selector")
                    {
                        GridBoard.Children.RemoveAt(i);
                        break;
                    }
                }
            }

            if (command.Contains("mark_cells:"))
            {
                var line = command.Remove(0, 12).Split(" ");

                foreach (var name in line)
                {
                    var selectedFigure = new Rectangle()
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
            {
                for (int i = GridBoard.Children.Count - 1; i >= 0; --i)
                {
                    if (GridBoard.Children[i] is Rectangle cellMarker && (string) cellMarker.Tag == "marker")
                    {
                        GridBoard.Children.RemoveAt(i);
                    }
                }
            }

            if (command.Contains("move:"))
            {
                var line = command.Split(" ");
                var nameFrom = line[1];
                var nameTo = line[2];
                var color = line[3];

                for (int i = GridBoard.Children.Count - 1; i >= 0; --i)
                {
                    if (GridBoard.Children[i] is Ellipse figure && figure.Name == nameFrom)
                    {
                        GridBoard.Children.RemoveAt(i);
                        break;
                    }
                }
                
                Execute("unselect");

                var checker = new Ellipse()
                {
                    Fill = (color == "Black") ? Brushes.Goldenrod : Brushes.Silver,
                    Width = 40,
                    Height = 40,
                    IsHitTestVisible = false,
                    Name = $"{nameTo.First()}{nameTo.Last()}"
                };

                Grid.SetColumn(checker, nameTo.First() - 'a' + 1);
                Grid.SetRow(checker, '8' - nameTo.Last());

                GridBoard.Children.Add(checker);
                
                Execute($"select_figure: {nameTo}");
            }
        }
    }
}