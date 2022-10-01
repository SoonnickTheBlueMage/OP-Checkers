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
 *    7 b_b_b_b_                        8 b_b_b_b_
 *    6 _b_b_b_b                        7 _b_b_b_b
 *    5 b_b_b_b_                        6 b_b_b_b_
 *    4 ________                        5 ________
 *    3 ________                        4 ________
 *    2 _w_w_w_w                        3 _w_w_w_w
 *    1 w_w_w_w_                        2 w_w_w_w_
 *    0 _w_w_w_w                        1 _w_w_w_w
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

                if (rowIterator % 2 != columnIterator % 2)
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

    public Figure? Cell(char column, int row) // доступ к клетке по visual координатам 
    {
        var prepColumn = VisualColumn2BoardColumn(column);
        var prepRow = VisualRow2BoardRow(row);

        return _board[prepRow][prepColumn];
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
                    // todo if its Quenn
                }
            }
        }

        return mustAttack.Count != 0 ? mustAttack : basicMoves;
    }
}