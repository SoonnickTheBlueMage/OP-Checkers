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
    private Tuple<char, int>? _picked = null;

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
        var command = "mark_cells: ";

        foreach (var line in possiblePick)
        {
            command += line.Item1.ToString() + line.Item2.ToString() + " ";
        }

        return command[..^1];
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

            if (!possiblePick.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                return new List<string>();


            _picked = new Tuple<char, int>(cellColumn, cellRow);
            _turnStatus = TurnStatus.WaitingMoveToCellPick;
            
            var possibleBasicMoves = _gameBoard.FigureCanBasicMove(_picked.Item1, _picked.Item2);
            var command = "mark_cells: ";
            foreach (var line in possibleBasicMoves)
                command += line.Item1.ToString() + line.Item2.ToString() + " ";
            command = command[..^1];

            return new List<string> {"unmark_cells", $"select_figure: {cellColumn}{cellRow}", command};
        }
        else
        {
            if (new Tuple<char, int>(cellColumn, cellRow).Equals(_picked))
            {
                _picked = null;
                _turnStatus = TurnStatus.WaitingFigurePick;
                
                return new List<string>() {"unselect", DrawPossiblePickCommand()};
            }

            if (_picked == null)
                return new List<string>();

            var possibleBasicMoves = _gameBoard.FigureCanBasicMove(_picked.Item1, _picked.Item2);
            
            if (! possibleBasicMoves.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                return new List<string> {$"Message: чел ты {possibleBasicMoves.Count}"};
            
            _gameBoard.MoveFigure(_picked.Item1, _picked.Item2, cellColumn, cellRow);

            var command = $"move: {_picked.Item1}{_picked.Item2} {cellColumn}{cellRow} {_turnColor}";
            _picked = new Tuple<char, int>(cellColumn, cellRow);
            
            //if stil must attack must attack dont end turn
            return new List<string> {"unmark_cells", command};
        }
    }
}