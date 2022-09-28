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

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CreateTable();
            AddCheckers();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Кнопка {(sender as Button)?.Name} нажата");
        }

        private void CreateTable()
        {
            for (var rowIterator = 0; rowIterator < 8; rowIterator++)
            {
                for (var columnIterator = 1; columnIterator < 9; columnIterator++)
                {
                    if (rowIterator % 2 == columnIterator % 2)
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
                    if ((rowIterator % 2 != columnIterator % 2) && rowIterator is < 3 or > 4)
                    {
                        var checker = new Ellipse()
                        {
                            Fill = (rowIterator < 3) ? Brushes.Goldenrod : Brushes.Silver,
                            Width = 40,
                            Height = 40,
                            IsHitTestVisible = false
                        };

                        Grid.SetColumn(checker, columnIterator);
                        Grid.SetRow(checker, rowIterator);

                        GridBoard.Children.Add(checker);
                    }
                }
            }
        }
    }
}