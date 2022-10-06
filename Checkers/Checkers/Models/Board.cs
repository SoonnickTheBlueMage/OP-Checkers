using System;
using System.Collections.Generic;

namespace Checkers.Models;

/*
 *  Этот класс используется для выполнения всех проверок и перемещений.
 *  Он полностью соответствует тому что рисуется на доске в окне, но
 *  все операции надо выполнять как в нем, так и в grid окна.
 *
 *  Индекс рядов в этом классе совпадает с grid.
 *  Индекс колонн в этом классе на 1 меньше чем в grid
 *
 *
 *  Board Coordinates               Visual Coordinates
 *  (координаты _board)         (координаты - имена кнопок) 
 *                                vertical (a-h) - column
 *                                horizontal (1-8) - row
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

    public Board() // заполняю доску при создании
    {
        for (var rowIterator = 0; rowIterator < 8; rowIterator++)
        {
            _board.Add(new List<Figure?>());

            for (var columnIterator = 0; columnIterator < 8; columnIterator++)
            {
                Figure? currentFigure = null;

                if (rowIterator % 2 == columnIterator % 2)
                    currentFigure = rowIterator switch
                    {
                        < 3 => new Figure(Color.White),
                        > 4 => new Figure(Color.Black),
                        _ => null
                    };

                _board[rowIterator].Add(currentFigure);
            }
        }
    }

    private static int VisRow2BoardRow(int visRow)
    {
        return visRow - 1;
    }

    private static int VisCol2BoardCol(char visCol)
    {
        return visCol - 'a';
    }

    private static int BoardRow2VisRow(int boardRow)
    {
        return boardRow + 1;
    }

    private static char BoardCol2VisCol(int boardColumn)
    {
        return (char) (boardColumn + 'a');
    }

    private List<Tuple<char, int>> WhereCheckerCanMove(int boardCol, int boardRow)
    {
        if (_board[boardRow][boardCol] == null)
            return new List<Tuple<char, int>>();

        var playingColor = _board[boardRow][boardCol]!.GetColor();
        var moves = new List<Tuple<char, int>>();

        if (playingColor == Color.White &&
            boardRow + 1 < 8 &&
            boardCol + 1 < 8 &&
            _board[boardRow + 1][boardCol + 1] == null)
            moves.Add(new Tuple<char, int>
            (BoardCol2VisCol(boardCol + 1),
                BoardRow2VisRow(boardRow + 1)));

        if (playingColor == Color.White &&
            boardRow + 1 < 8 &&
            boardCol - 1 >= 0 &&
            _board[boardRow + 1][boardCol - 1] == null)
            moves.Add(new Tuple<char, int>
            (BoardCol2VisCol(boardCol - 1),
                BoardRow2VisRow(boardRow + 1)));

        if (playingColor == Color.Black &&
            boardRow - 1 >= 0 &&
            boardCol + 1 < 8 &&
            _board[boardRow - 1][boardCol + 1] == null)
            moves.Add(new Tuple<char, int>
            (BoardCol2VisCol(boardCol + 1),
                BoardRow2VisRow(boardRow - 1)));

        if (playingColor == Color.Black &&
            boardRow - 1 >= 0 &&
            boardCol - 1 >= 0 &&
            _board[boardRow - 1][boardCol - 1] == null)
            moves.Add(new Tuple<char, int>
            (BoardCol2VisCol(boardCol - 1),
                BoardRow2VisRow(boardRow - 1)));

        return moves;
    }

    private List<Tuple<char, int>> WhereCheckerCanAttack(int boardCol, int boardRow)
    {
        if (_board[boardRow][boardCol] == null)
            return new List<Tuple<char, int>>();

        var playingColor = _board[boardRow][boardCol]!.GetColor();
        var attacks = new List<Tuple<char, int>>();

        if (boardRow + 1 < 8 && boardCol - 1 >= 0 &&
            _board[boardRow + 1][boardCol - 1] != null &&
            _board[boardRow + 1][boardCol - 1]!.GetColor() != playingColor &&
            boardRow + 2 < 8 && boardCol - 2 >= 0 &&
            _board[boardRow + 2][boardCol - 2] == null)
            attacks.Add(new Tuple<char, int>(BoardCol2VisCol(boardCol - 2),
                BoardRow2VisRow(boardRow + 2)));

        if (boardRow + 1 < 8 && boardCol + 1 < 8 &&
            _board[boardRow + 1][boardCol + 1] != null &&
            _board[boardRow + 1][boardCol + 1]!.GetColor() != playingColor &&
            boardRow + 2 < 8 && boardCol + 2 < 8 &&
            _board[boardRow + 2][boardCol + 2] == null)
            attacks.Add(new Tuple<char, int>(BoardCol2VisCol(boardCol + 2),
                BoardRow2VisRow(boardRow + 2)));

        if (boardRow - 1 >= 0 && boardCol - 1 >= 0 &&
            _board[boardRow - 1][boardCol - 1] != null &&
            _board[boardRow - 1][boardCol - 1]!.GetColor() != playingColor &&
            boardRow - 2 >= 0 && boardCol - 2 >= 0 &&
            _board[boardRow - 2][boardCol - 2] == null)
            attacks.Add(new Tuple<char, int>(BoardCol2VisCol(boardCol - 2),
                BoardRow2VisRow(boardRow - 2)));

        if (boardRow - 1 >= 0 && boardCol + 1 < 8 &&
            _board[boardRow - 1][boardCol + 1] != null &&
            _board[boardRow - 1][boardCol + 1]!.GetColor() != playingColor &&
            boardRow - 2 >= 0 && boardCol + 2 < 8 &&
            _board[boardRow - 2][boardCol + 2] == null)
            attacks.Add(new Tuple<char, int>(BoardCol2VisCol(boardCol + 2),
                BoardRow2VisRow(boardRow - 2)));

        return attacks;
    }

    private List<Tuple<char, int>> WhereQueenCanMove(int boardCol, int boardRow)
    {
        if (_board[boardRow][boardCol] == null)
            return new List<Tuple<char, int>>();

        var playingColor = _board[boardRow][boardCol]!.GetColor();
        var moves = new List<Tuple<char, int>>();

        var meetEnemy = false;
        for (var iter = 1; iter + boardRow < 8; iter++)
            if (iter + boardCol < 8)
            {
                if (_board[boardRow + iter][boardCol + iter] == null && !meetEnemy)
                {
                    moves.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol + iter),
                        BoardRow2VisRow(boardRow + iter)));
                }
                else
                {
                    if (meetEnemy)
                        break;

                    if (_board[boardRow + iter][boardCol + iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        meetEnemy = false;
        for (var iter = 1; iter + boardRow < 8; iter++)
            if (boardCol - iter >= 0)
            {
                if (_board[boardRow + iter][boardCol - iter] ==
                    null && !meetEnemy)
                {
                    moves.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol - iter),
                        BoardRow2VisRow(boardRow + iter)));
                }
                else
                {
                    if (meetEnemy)
                        break;

                    if (_board[boardRow + iter][boardCol - iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        meetEnemy = false;
        for (var iter = 1; boardRow - iter >= 0; iter++)
            if (iter + boardCol < 8)
            {
                if (_board[boardRow - iter][boardCol + iter] ==
                    null && !meetEnemy)
                {
                    moves.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol + iter),
                        BoardRow2VisRow(boardRow - iter)));
                }
                else
                {
                    if (meetEnemy)
                        break;

                    if (_board[boardRow - iter][boardCol + iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        meetEnemy = false;
        for (var iter = 1; boardRow - iter >= 0; iter++)
            if (boardCol - iter >= 0)
            {
                if (_board[boardRow - iter][boardCol - iter] ==
                    null && !meetEnemy)
                {
                    moves.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol - iter),
                        BoardRow2VisRow(boardRow - iter)));
                }
                else
                {
                    if (meetEnemy)
                        break;

                    if (_board[boardRow - iter][boardCol - iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        return moves;
    }

    private List<Tuple<char, int>> WhereQueenCanAttack(int boardCol, int boardRow)
    {
        if (_board[boardRow][boardCol] == null)
            return new List<Tuple<char, int>>();

        var playingColor = _board[boardRow][boardCol]!.GetColor();
        var attacks = new List<Tuple<char, int>>();

        var meetEnemy = false;
        for (var iter = 1; iter + boardRow < 8; iter++)
            if (iter + boardCol < 8)
            {
                if (_board[boardRow + iter][boardCol + iter] == null && meetEnemy)
                {
                    attacks.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol + iter),
                        BoardRow2VisRow(boardRow + iter)));
                }
                else
                {
                    if (_board[boardRow + iter][boardCol + iter] == null)
                        continue;

                    if (meetEnemy) break;

                    if (_board[boardRow + iter][boardCol + iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        meetEnemy = false;
        for (var iter = 1; iter + boardRow < 8; iter++)
            if (boardCol - iter >= 0)
            {
                if (_board[boardRow + iter][boardCol - iter] ==
                    null && meetEnemy)
                {
                    attacks.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol - iter),
                        BoardRow2VisRow(boardRow + iter)));
                }
                else
                {
                    if (_board[boardRow + iter][boardCol - iter] == null)
                        continue;

                    if (meetEnemy) break;

                    if (_board[boardRow + iter][boardCol - iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        meetEnemy = false;
        for (var iter = 1; boardRow - iter >= 0; iter++)
            if (iter + boardCol < 8)
            {
                if (_board[boardRow - iter][boardCol + iter] ==
                    null && meetEnemy)
                {
                    attacks.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol + iter),
                        BoardRow2VisRow(boardRow - iter)));
                }
                else
                {
                    if (_board[boardRow - iter][boardCol + iter] == null)
                        continue;

                    if (meetEnemy) break;

                    if (_board[boardRow - iter][boardCol + iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        meetEnemy = false;
        for (var iter = 1; boardRow - iter >= 0; iter++)
            if (boardCol - iter >= 0)
            {
                if (_board[boardRow - iter][boardCol - iter] ==
                    null && meetEnemy)
                {
                    attacks.Add(new Tuple<char, int>(
                        BoardCol2VisCol(boardCol - iter),
                        BoardRow2VisRow(boardRow - iter)));
                }
                else
                {
                    if (_board[boardRow - iter][boardCol - iter] == null)
                        continue;

                    if (meetEnemy)
                        break;

                    if (_board[boardRow - iter][boardCol - iter]!.GetColor() != playingColor)
                        meetEnemy = true;
                    else
                        break;
                }
            }
            else
            {
                break;
            }

        return attacks;
    }

    public Figure? Cell(char visCol, int visRow) // доступ к клетке
    {
        return _board[VisRow2BoardRow(visRow)][VisCol2BoardCol(visCol)];
    }

    public List<Tuple<char, int>> AllPossibleToPick(Color playingColor) // кем можно ходить (учитывая "бить обязан")
    {
        var basicMoves = new List<Tuple<char, int>>();
        var mustAttack = new List<Tuple<char, int>>();

        for (var rowIterator = 0; rowIterator < 8; rowIterator++)
        for (var columnIterator = 0; columnIterator < 8; columnIterator++)
        {
            var cell = _board[rowIterator][columnIterator];

            if (cell == null)
                continue;

            if (cell.GetColor() != playingColor)
                continue;

            if (cell.GetStatus() == Status.Checker)
            {
                if (WhereCheckerCanMove(columnIterator, rowIterator).Count > 0)
                    basicMoves.Add(new Tuple<char, int>(BoardCol2VisCol(columnIterator), BoardRow2VisRow(rowIterator)));

                if (WhereCheckerCanAttack(columnIterator, rowIterator).Count > 0)
                    mustAttack.Add(new Tuple<char, int>(BoardCol2VisCol(columnIterator), BoardRow2VisRow(rowIterator)));
            }
            else
            {
                if (WhereQueenCanMove(columnIterator, rowIterator).Count > 0)
                    basicMoves.Add(new Tuple<char, int>(BoardCol2VisCol(columnIterator), BoardRow2VisRow(rowIterator)));

                if (WhereQueenCanAttack(columnIterator, rowIterator).Count > 0)
                    mustAttack.Add(new Tuple<char, int>(BoardCol2VisCol(columnIterator), BoardRow2VisRow(rowIterator)));
            }
        }

        return mustAttack.Count != 0 ? mustAttack : basicMoves;
    }

    public List<Tuple<char, int>> WhereFigureCanMove(char visCol, int visRow)
    {
        var basicMoves = new List<Tuple<char, int>>();

        var figure = Cell(visCol, visRow);

        if (figure == null)
            return new List<Tuple<char, int>>();

        if (figure.GetStatus() == Status.Checker)
            basicMoves.AddRange(WhereCheckerCanMove(VisCol2BoardCol(visCol), VisRow2BoardRow(visRow)));
        else
            basicMoves.AddRange(WhereQueenCanMove(VisCol2BoardCol(visCol), VisRow2BoardRow(visRow)));

        return basicMoves;
    }

    public List<Tuple<char, int>> WhereFigureCanAttack(char visCol, int visRow)
    {
        var mustAttack = new List<Tuple<char, int>>();

        var figure = Cell(visCol, visRow);

        if (figure == null)
            return new List<Tuple<char, int>>();

        if (figure.GetStatus() == Status.Checker)
            mustAttack.AddRange(WhereCheckerCanAttack(VisCol2BoardCol(visCol), VisRow2BoardRow(visRow)));
        else
            mustAttack.AddRange(WhereQueenCanAttack(VisCol2BoardCol(visCol), VisRow2BoardRow(visRow)));

        return mustAttack;
    }

    public bool MoveFigure(char visColumnFrom, int visRowFrom, char visColumnTo, int visRowTo)
    {
        // Перед использованием выполнить все необходимые проверки логики.

        if (Cell(visColumnFrom, visRowFrom) == null || Cell(visColumnTo, visRowTo) != null)
            return false;

        var transform = false;

        if (Cell(visColumnFrom, visRowFrom)!.GetStatus() == Status.Checker)
        {
            if (Cell(visColumnFrom, visRowFrom)!.GetColor() == Color.White && VisRow2BoardRow(visRowTo) == 7)
            {
                Cell(visColumnFrom, visRowFrom)!.TransformToQueen();
                transform = true;
            }

            if (Cell(visColumnFrom, visRowFrom)!.GetColor() == Color.Black && VisRow2BoardRow(visRowTo) == 0)
            {
                Cell(visColumnFrom, visRowFrom)!.TransformToQueen();
                transform = true;
            }
        }

        _board[VisRow2BoardRow(visRowTo)][VisCol2BoardCol(visColumnTo)] =
            _board[VisRow2BoardRow(visRowFrom)][VisCol2BoardCol(visColumnFrom)];

        _board[VisRow2BoardRow(visRowFrom)][VisCol2BoardCol(visColumnFrom)] = null;

        return transform;
    }

    public void DeleteFigure(char visCol, int visRow)
    {
        _board[VisRow2BoardRow(visRow)][VisCol2BoardCol(visCol)] = null;
    }

    public void AddFigure(char visCol, int visRow, Color color, Status status)
    {
        _board[VisRow2BoardRow(visRow)][VisCol2BoardCol(visCol)] = new Figure(color);

        if (status == Status.Queen)
            _board[VisRow2BoardRow(visRow)][VisCol2BoardCol(visCol)]!.TransformToQueen();
    }

    public Tuple<char, int> GetKilledFiguresCell(char visColumnFrom, int visRowFrom, char visColumnTo, int visRowTo)
    {
        // Использовать до MoveFigure и DeleteFigure !!!
        // Неправильное использование ведет к падению кода, поэтому
        // применять с гарантированно верными параметрами, в нужном месте, чтобы не было ссылок на пустоту.

        var killedColumn = 'a';
        var killedRow = 1;

        if (Cell(visColumnFrom, visRowFrom) != null)
            if (Cell(visColumnFrom, visRowFrom)!.GetStatus() == Status.Checker)
            {
                killedColumn = BoardCol2VisCol((VisCol2BoardCol(visColumnTo) -
                                                VisCol2BoardCol(visColumnFrom)) / 2 +
                                               VisCol2BoardCol(visColumnFrom));
                killedRow = BoardRow2VisRow((VisRow2BoardRow(visRowTo) - VisRow2BoardRow(visRowFrom)) / 2 +
                                            VisRow2BoardRow(visRowFrom));
            }
            else
            {
                var signCol = visColumnTo > visColumnFrom ? 1 : -1;
                var signRow = visRowTo > visRowFrom ? 1 : -1;

                for (var iter = 0;
                     VisCol2BoardCol(visColumnFrom) + iter * signCol is >= 0 and < 8 &&
                     VisRow2BoardRow(visRowFrom) + iter * signRow is >= 0 and < 8;
                     iter++)
                    if (_board[VisRow2BoardRow(visRowFrom) + iter * signRow]
                            [VisCol2BoardCol(visColumnFrom) + iter * signCol] != null &&
                        _board[VisRow2BoardRow(visRowFrom) + iter * signRow][
                            VisCol2BoardCol(visColumnFrom) + iter * signCol]!.GetColor() !=
                        _board[VisRow2BoardRow(visRowFrom)][VisCol2BoardCol(visColumnFrom)]!.GetColor())
                    {
                        killedColumn =
                            BoardCol2VisCol(VisCol2BoardCol(visColumnFrom) + iter * signCol);
                        killedRow = BoardRow2VisRow(VisRow2BoardRow(visRowFrom) + iter * signRow);
                        break;
                    }
            }

        return new Tuple<char, int>(killedColumn, killedRow);
    }
}