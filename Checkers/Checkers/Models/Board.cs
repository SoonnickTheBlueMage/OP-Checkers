using System.Collections.Generic;

namespace Checkers.Models;

public class Board
{
    private List<List<Figure?>> _board = new();

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

    public Figure? Cell(char column, int row) // доступ к клетке по координатам 
    {
        var prepColumn = column - 'a';
        var prepRow = row - 1;

        return _board[prepColumn][prepRow];
    }

    public bool Move(char columnFrom, int rowFrom, char columnTo, int rowTo) // перемещение фигурки, true - все ок
    {
        var prepColumnFrom = columnFrom - 'a';
        var prepRowFrom = rowFrom - 1;
        
        var prepColumnTo = columnTo - 'a';
        var prepRowTo = rowTo - 1;

        if (prepColumnFrom % 2 == prepRowFrom % 2 || prepColumnTo % 2 == prepRowTo % 2)
        {
            // клетка не черная
            return false;
        }

        if (prepColumnFrom is < 0 or > 7 || prepRowFrom is < 0 or > 7 ||
            prepColumnTo is < 0 or > 7 || prepRowTo is < 0 or > 7)
        {
            // неправильные координаты
            return false;
        }

        if (Cell(columnFrom, rowFrom) == null)
        {
            // клетка, из которой ходим, пустая
            return false;
        }

        if (Cell(columnTo, rowTo) != null)
        {
            // клутка, в которую ходим, не пустая
            return false;
        }
        
        var movingFigure =  Cell(columnFrom, rowFrom); // помнить что класс - ссылочный тип
        _board[prepColumnFrom][prepRowFrom] = null;
        _board[prepColumnTo][prepRowTo] = movingFigure;
        
        /* На потом
        if (prepRowTo == 7 && movingFigure!.GetColor() == Color.White ||
            prepRowTo == 0 && movingFigure!.GetColor() == Color.Black)
            movingFigure.SetStatus(Status.Queen);
        */

        return true;
    }
}