using System;
using System.Collections.Generic; // SOLID?

namespace Checkers.Models;

public enum TurnStatus
{
    WaitingFigurePick,
    WaitingMoveToCellPick
}

public class Game
{
    private Color _turnColor;
    private readonly Board _gameBoard;
    private int _whiteFigures;
    private int _blackFigures;
    private TurnStatus _turnStatus;
    private Tuple<char, int>? _picked;
    
    //private List<string> turnLogger;


    private bool GameContinues()
    {
        return _whiteFigures > 0 && _blackFigures > 0;
    } //игра не закончилась?

    private void NextTurn()
    {
        _turnColor = _turnColor == Color.White ? Color.Black : Color.White;
    } // смена ходящего игрока

    public Game()
    {
        _turnColor = Color.White;
        _gameBoard = new Board();
        _whiteFigures = 12;
        _blackFigures = 12;
        _turnStatus = TurnStatus.WaitingFigurePick;
        _picked = null;
    } // constructor

    public string DrawPossiblePickCommand()
    {
        var possiblePick = _gameBoard.AllPossibleToPick(_turnColor);
        var command = "enlight cell: ";
        
        foreach (var line in possiblePick)
        {
            command += line.Item1.ToString() + line.Item2.ToString() + " ";
        }

        return command;
    }

    public List<string> Turn(char cellColumn, int cellRow)
    {
        if (!GameContinues())
        {
            return new List<string> {"message: Game over"};
        }
        
        if (_turnStatus == TurnStatus.WaitingFigurePick)
        {
            var possiblePick = _gameBoard.AllPossibleToPick(_turnColor);
            
            if (_gameBoard.Cell(cellColumn, cellRow) == null)
            {
                // клетка пустая
                return new List<string>();
            }

            else if (! possiblePick.Contains(new Tuple<char, int>(cellColumn, cellRow)))
            {
                // нельзя пикнуть
                return new List<string>();
            }
            else
            {
                _picked = new Tuple<char, int>(cellColumn, cellRow);
                _turnStatus = TurnStatus.WaitingMoveToCellPick;

                return new List<string> {$"set figure: {cellColumn}{cellRow}  {_gameBoard.Cell(cellColumn, cellRow).GetColor()}"};
            }
        }
        
        // elsr if _turn status = ...
        
        return new List<string>();
    }
}