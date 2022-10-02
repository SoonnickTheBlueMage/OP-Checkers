using System;
using System.Collections.Generic;

// SOLID?

namespace Checkers.Models;

public enum TurnStatus
{
    WaitingFigurePick,
    WaitingMoveToCellPick
}

public class Game
{
    private readonly Board _gameBoard;
    private int _blackFigures;
    private Tuple<char, int>? _picked;
    private Color _turnColor;
    private TurnStatus _turnStatus;
    private int _whiteFigures;

    public Game()
    {
        _turnColor = Color.White;
        _gameBoard = new Board();
        _whiteFigures = 12;
        _blackFigures = 12;
        _turnStatus = TurnStatus.WaitingFigurePick;
        _picked = null;
    } // constructor

    //private List<string> turnLogger;

    //нет правила про то что надо бить большинство

    private bool GameContinues()
    {
        return _whiteFigures > 0 && _blackFigures > 0;
    } //игра не закончилась?

    private void NextTurn()
    {
        _turnColor = _turnColor == Color.White ? Color.Black : Color.White;
        _turnStatus = TurnStatus.WaitingFigurePick;
        _picked = null;
    } // смена ходящего игрока

    private string DrawPossibleAttackCommand(char column, int row)
    {
        if (_picked == null)
            return "";

        var possibleAttacks = _gameBoard.FigureCanAttack(_picked.Item1, _picked.Item2, _turnColor);
        var command = "mark_cells: ";

        foreach (var line in possibleAttacks)
            command += line.Item1 + line.Item2.ToString() + " ";

        return command[..^1];
    }

    public string DrawPossiblePickCommand()
    {
        var possiblePick = _gameBoard.AllPossibleToPick(_turnColor);
        var command = "mark_cells: ";

        foreach (var line in possiblePick)
            command += line.Item1 + line.Item2.ToString() + " ";

        return command[..^1];
    }

    public List<string> Turn(char cellColumn, int cellRow)
    {
        if (!GameContinues())
            return new List<string> {$"message: Game over, {(_whiteFigures == 0 ? "Black win" : "White win")}"};

        if (_turnStatus == TurnStatus.WaitingFigurePick)
        {
            var possiblePick = _gameBoard.AllPossibleToPick(_turnColor);

            if (!possiblePick.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                return new List<string>();

            _picked = new Tuple<char, int>(cellColumn, cellRow);
            _turnStatus = TurnStatus.WaitingMoveToCellPick;

            var possibleBasicMoves = _gameBoard.FigureCanBasicMove(_picked.Item1, _picked.Item2);
            var possibleAttacks = _gameBoard.FigureCanAttack(_picked.Item1, _picked.Item2, _turnColor);

            var command = "mark_cells: ";
            foreach (var line in possibleAttacks.Count > 0 ? possibleAttacks : possibleBasicMoves)
                command += line.Item1 + line.Item2.ToString() + " ";
            command = command[..^1];

            return new List<string> {"unmark_cells", $"select_figure: {cellColumn}{cellRow}", command};
        }
        else
        {
            if (new Tuple<char, int>(cellColumn, cellRow).Equals(_picked))
            {
                _picked = null;
                _turnStatus = TurnStatus.WaitingFigurePick;

                return new List<string> {"unselect", "unmark_cells", DrawPossiblePickCommand()};
            }

            if (_picked == null)
                return new List<string>();

            var transform = false;
            var possibleBasicMoves = _gameBoard.FigureCanBasicMove(_picked.Item1, _picked.Item2);
            var possibleAttacks = _gameBoard.FigureCanAttack(_picked.Item1, _picked.Item2, _turnColor);

            if (possibleAttacks.Count > 0)
            {
                if (!possibleAttacks.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                    return new List<string>();

                var killed = _gameBoard.GetKilledFiguresCell(
                    _picked.Item1, _picked.Item2, cellColumn, cellRow);

                var moveCommand = $"move: {_picked.Item1}{_picked.Item2} {cellColumn}{cellRow} {_turnColor} " +
                                  $"{_gameBoard.Cell(_picked.Item1, _picked.Item2)!.GetStatus()}";
                var killCommand = $"erase: {killed.Item1}{killed.Item2}";

                transform = _gameBoard.MoveFigure(_picked.Item1, _picked.Item2, cellColumn, cellRow);
                _gameBoard.DeleteFigure(killed.Item1, killed.Item2);

                _picked = new Tuple<char, int>(cellColumn, cellRow);

                if (_turnColor == Color.White)
                    _blackFigures--;
                else
                    _whiteFigures--;

                var returnCommandList = new List<string> {"unmark_cells", moveCommand, killCommand};

                if (transform) returnCommandList.Add($"transform: {_picked.Item1}{_picked.Item2} {_turnColor}");

                if (_gameBoard.FigureCanAttack(_picked.Item1, _picked.Item2, _turnColor).Count == 0)
                {
                    NextTurn();
                    returnCommandList.Add("unselect");
                    returnCommandList.Add(DrawPossiblePickCommand());
                }
                else
                {
                    returnCommandList.Add(DrawPossibleAttackCommand(_picked.Item1, _picked.Item2));
                }

                return returnCommandList;
            }

            if (!possibleBasicMoves.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                return new List<string>();

            var command = $"move: {_picked.Item1}{_picked.Item2} {cellColumn}{cellRow} {_turnColor} " +
                          $"{_gameBoard.Cell(_picked.Item1, _picked.Item2)!.GetStatus()}";

            transform = _gameBoard.MoveFigure(_picked.Item1, _picked.Item2, cellColumn, cellRow);

            if (!possibleBasicMoves.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                return new List<string>();

            var returnCommands = new List<string> {"unmark_cells", command, "unselect"};

            if (transform) returnCommands.Add($"transform: {cellColumn}{cellRow} {_turnColor}");

            NextTurn();

            returnCommands.Add(DrawPossiblePickCommand());

            return returnCommands;
        }
    }
}