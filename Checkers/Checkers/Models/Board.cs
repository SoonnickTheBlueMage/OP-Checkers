using System.Collections.Generic;

namespace Checkers.Models;

public class Board
{
    private List<List<Figure?>> _board;

    public Board()
    {
        for (var i = 0; i < 8; i++)
        {
            _board.Add(new List<Figure?>());
            
            for (var j = 0; j < 8; j++)
            {
                _board
            }
        }
    }
}