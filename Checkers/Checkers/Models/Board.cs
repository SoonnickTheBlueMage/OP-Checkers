using System;
using System.Collections.Generic; // нарушение SOLID?

namespace Checkers.Models;

/*
 * этот класс используется для выполнения всех проверок и перемешений
 * он полностью соответствует тому что рисуется в окне, но
 * все операции надо выполнять как в нем, так и в grid окна
 *
 * индекс рядов в этом классе совпадает с grid
 * индекс колонн в этом классе на 1 меньше чем в grid
 */
/*
 *  Board Coordinates               Visual Coordinates
 *  horizontal - column
 *  vertical - row
 * 
 *      01234567                          abcdefgh
 *    7 _b_b_b_b                        8 _b_b_b_b
 *    6 b_b_b_b_                        7 b_b_b_b_
 *    5 _b_b_b_b                        6 _b_b_b_b
 *    4 ________                        5 ________
 *    3 ________                        4 ________
 *    2 w_w_w_w_                        3 w_w_w_w_
 *    1 _w_w_w_w                        2 _w_w_w_w
 *    0 w_w_w_w_                        1 w_w_w_w_
 */

public class Board
{
    private readonly List<List<Figure?>> _board = new();

    private static int VisualRow2BoardRow(int visualRow)
    {
        return visualRow - 1;
    }

    private static int VisualColumn2BoardColumn(char visualColumn)
    {
        return visualColumn - 'a';
    }

    private static int BoardRow2VisualRow(int boardRow)
    {
        return boardRow + 1;
    }

    private static char BoardColumn2VisualColumn(int boardColumn)
    {
        return (char) (boardColumn + 'a');
    }


    public Board() // заполняю доску при создании
    {
        for (var rowIterator = 0; rowIterator < 8; rowIterator++)
        {
            _board.Add(new List<Figure?>());

            for (var columnIterator = 0; columnIterator < 8; columnIterator++)
            {
                Figure? currentFigure = null;

                if (rowIterator % 2 == columnIterator % 2)
                {
                    currentFigure = rowIterator switch
                    {
                        < 3 => new Figure(Color.White),
                        > 4 => new Figure(Color.Black),
                        _ => null
                    };
                }

                _board[rowIterator].Add(currentFigure);
            }
        }
    }

    public Figure? Cell(char visualColumn, int visualRow) // доступ к клетке
    {
        return _board[VisualRow2BoardRow(visualRow)][VisualColumn2BoardColumn(visualColumn)];
    }

    public List<Tuple<char, int>> AllPossibleToPick(Color playingColor)
    {
        var basicMoves = new List<Tuple<char, int>>();
        var mustAttack = new List<Tuple<char, int>>();

        for (var rowIterator = 0; rowIterator < 8; rowIterator++)
        {
            for (var columnIterator = 0; columnIterator < 8; columnIterator++)
            {
                var cell = _board[rowIterator][columnIterator];

                if (cell == null)
                    continue;

                if (cell.GetColor() != playingColor)
                    continue;

                if (cell.GetStatus() == Status.Checker)
                {
                    //check basic moves
                    if (playingColor == Color.White && rowIterator + 1 < 8 &&
                        (columnIterator + 1 < 8 && _board[rowIterator + 1][columnIterator + 1] == null ||
                         columnIterator - 1 >= 0 && _board[rowIterator + 1][columnIterator - 1] == null))
                    {
                        basicMoves.Add(new Tuple<char, int>(BoardColumn2VisualColumn(columnIterator),
                            BoardRow2VisualRow(rowIterator)));
                    }

                    if (playingColor == Color.Black && rowIterator - 1 >= 0 &&
                        (columnIterator + 1 < 8 && _board[rowIterator - 1][columnIterator + 1] == null ||
                         columnIterator - 1 >= 0 && _board[rowIterator - 1][columnIterator - 1] == null))
                    {
                        basicMoves.Add(new Tuple<char, int>(BoardColumn2VisualColumn(columnIterator),
                            BoardRow2VisualRow(rowIterator)));
                    }
                    
                    //check must atack
                    if (rowIterator + 1 < 8 && columnIterator - 1 >= 0 &&
                        _board[rowIterator + 1][columnIterator - 1] != null &&
                        _board[rowIterator + 1][columnIterator - 1]!.GetColor() != playingColor &&
                        rowIterator + 2 < 8 && columnIterator - 2 >= 0 &&
                        _board[rowIterator + 2][columnIterator - 2] == null)
                    {
                        mustAttack.Add(new Tuple<char, int>(BoardColumn2VisualColumn(columnIterator),
                            BoardRow2VisualRow(rowIterator)));
                    }

                    if (rowIterator + 1 < 8 && columnIterator + 1 < 8 &&
                        _board[rowIterator + 1][columnIterator + 1] != null &&
                        _board[rowIterator + 1][columnIterator + 1]!.GetColor() != playingColor &&
                        rowIterator + 2 < 8 && columnIterator + 2 < 8 &&
                        _board[rowIterator + 2][columnIterator + 2] == null)
                    {
                        mustAttack.Add(new Tuple<char, int>(BoardColumn2VisualColumn(columnIterator),
                            BoardRow2VisualRow(rowIterator)));
                    }

                    if (rowIterator - 1 >= 0 && columnIterator - 1 >= 0 &&
                        _board[rowIterator - 1][columnIterator - 1] != null &&
                        _board[rowIterator - 1][columnIterator - 1]!.GetColor() != playingColor &&
                        rowIterator - 2 >= 0 && columnIterator - 2 >= 0 &&
                        _board[rowIterator - 2][columnIterator - 2] == null)
                    {
                        mustAttack.Add(new Tuple<char, int>(BoardColumn2VisualColumn(columnIterator),
                            BoardRow2VisualRow(rowIterator)));
                    }

                    if (rowIterator - 1 >= 0 && columnIterator + 1 < 8 &&
                        _board[rowIterator - 1][columnIterator + 1] != null &&
                        _board[rowIterator - 1][columnIterator + 1]!.GetColor() != playingColor &&
                        rowIterator - 2 >= 0 && columnIterator + 2 < 8 &&
                        _board[rowIterator - 2][columnIterator + 2] == null)
                    {
                        mustAttack.Add(new Tuple<char, int>(BoardColumn2VisualColumn(columnIterator),
                            BoardRow2VisualRow(rowIterator)));
                    }
                }
                else
                {
                    // todo if it's Queen
                }
            }
        }

        return mustAttack.Count != 0 ? mustAttack : basicMoves;
    }

    public List<Tuple<char, int>> FigureCanBasicMove(char visualColumn, int visualRow)
    {
        var basicMoves = new List<Tuple<char, int>>();

        var figure = Cell(visualColumn, visualRow);

        if (figure == null)
            return new List<Tuple<char, int>>();

        if (figure.GetStatus() == Status.Checker)
        {
            if ( figure.GetColor() == Color.White && 
                 VisualRow2BoardRow(visualRow) + 1 < 8 &&
                 VisualColumn2BoardColumn(visualColumn) + 1 < 8 &&
                 _board[VisualRow2BoardRow(visualRow) + 1][VisualColumn2BoardColumn(visualColumn) + 1] == null)
            {
                basicMoves.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 1), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 1)));
            }

            if ( figure.GetColor() == Color.White && 
                 VisualRow2BoardRow(visualRow) + 1 < 8 &&
                 VisualColumn2BoardColumn(visualColumn) - 1 >= 0 &&
                 _board[VisualRow2BoardRow(visualRow) + 1][VisualColumn2BoardColumn(visualColumn) - 1] == null)
            {
                basicMoves.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 1), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 1)));
            }
            
            if ( figure.GetColor() == Color.Black && 
                 VisualRow2BoardRow(visualRow) - 1 >= 0 &&
                 VisualColumn2BoardColumn(visualColumn) + 1 < 8 &&
                 _board[VisualRow2BoardRow(visualRow) - 1][VisualColumn2BoardColumn(visualColumn) + 1] == null)
            {
                basicMoves.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 1), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 1)));
            }

            if ( figure.GetColor() == Color.Black && 
                 VisualRow2BoardRow(visualRow) - 1 >= 0 &&
                 VisualColumn2BoardColumn(visualColumn) - 1 >= 0 &&
                 _board[VisualRow2BoardRow(visualRow) - 1][VisualColumn2BoardColumn(visualColumn) - 1] == null)
            {
                basicMoves.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 1), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 1)));
            }
        }
        else
        {
            // todo if it's queen
        }
        
        return basicMoves;
    }

    public List<Tuple<char, int>> FigureCanAttack(char visualColumn, int visualRow, Color playingColor)
    {
        var mustAttack = new List<Tuple<char, int>>();

        var figure = Cell(visualColumn, visualRow);
        
        if (figure == null)
            return new List<Tuple<char, int>>();

        if (figure.GetStatus() == Status.Checker)
        {
            if ( VisualRow2BoardRow(visualRow) + 2 < 8 &&
                 VisualColumn2BoardColumn(visualColumn) + 2 < 8 &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 1), 
                      BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 1)) != null &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 1),
                     BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 1))!.GetColor() != playingColor &&
                 _board[VisualRow2BoardRow(visualRow) + 2][VisualColumn2BoardColumn(visualColumn) + 2] == null)
            {
                mustAttack.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 2), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 2)));
            }

            if ( VisualRow2BoardRow(visualRow) + 2 < 8 &&
                 VisualColumn2BoardColumn(visualColumn) - 2 >= 0 &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 1), 
                     BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 1)) != null &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 1),
                     BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 1))!.GetColor() != playingColor &&
                 _board[VisualRow2BoardRow(visualRow) + 2][VisualColumn2BoardColumn(visualColumn) - 2] == null)
            {
                mustAttack.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 2), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) + 2)));
            }
            
            if ( VisualRow2BoardRow(visualRow) - 2 >= 0 &&
                 VisualColumn2BoardColumn(visualColumn) + 2 < 8 &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 1), 
                     BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 1)) != null &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 1),
                     BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 1))!.GetColor() != playingColor &&
                 _board[VisualRow2BoardRow(visualRow) - 2][VisualColumn2BoardColumn(visualColumn) + 2] == null)
            {
                mustAttack.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) + 2), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 2)));
            }

            if ( VisualRow2BoardRow(visualRow) - 2 >= 0 &&
                 VisualColumn2BoardColumn(visualColumn) - 2 >= 0 &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 1), 
                     BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 1)) != null &&
                 Cell(BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 1),
                     BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 1))!.GetColor() != playingColor &&
                 _board[VisualRow2BoardRow(visualRow) - 2][VisualColumn2BoardColumn(visualColumn) - 2] == null)
            {
                mustAttack.Add(new Tuple<char, int>
                (BoardColumn2VisualColumn(VisualColumn2BoardColumn(visualColumn) - 2), 
                    BoardRow2VisualRow(VisualRow2BoardRow(visualRow) - 2)));
            }
        }
        else
        {
            //todo if it's queen
        }

        return mustAttack;
    }

    public void MoveFigure(char visColumnFrom, int visRowFrom, char visColumnTo, int visRowTo)
    {
        // this function is used only after check that the button pressed is in FigureCanMove list
        // so I dont write any checks here
        _board[VisualRow2BoardRow(visRowTo)][VisualColumn2BoardColumn(visColumnTo)] =
            _board[VisualRow2BoardRow(visRowFrom)][VisualColumn2BoardColumn(visColumnFrom)];

        _board[VisualRow2BoardRow(visRowFrom)][VisualColumn2BoardColumn(visColumnFrom)] = null;
    }
}