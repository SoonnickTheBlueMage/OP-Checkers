using System;
using System.Collections.Generic;
using System.Linq;

// SOLID?

namespace Checkers.Models;

public enum TurnStatus
{
    WaitingFigurePick,
    WaitingCellToMovePick,
    MultiAttack
}

public class Game
{
    private readonly Board _gameBoard;
    private int _blackFigures;
    private Tuple<char, int>? _picked;
    private Color _turnColor;
    private TurnStatus _turnStatus;
    private int _whiteFigures;
    private List<List<string>> _turnLogger;

    public Game()
    {
        _turnColor = Color.White;
        _gameBoard = new Board();
        _whiteFigures = 12;
        _blackFigures = 12;
        _turnStatus = TurnStatus.WaitingFigurePick;
        _picked = null;
        _turnLogger = new List<List<string>> {new()};
    } // constructor

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

    private void Log(string command)
    {
        if (command == "new_turn")
            _turnLogger.Add(new List<string>());
        else
            _turnLogger.Last().Add(command);
    }

    private List<string> UndoTurn()
    {
        if (_turnLogger.Count == 1)
            return new List<string>();

        var returnCommandList = new List<string> {"delete_last_log", "unmark_cells"};
        var thisTurn = _turnLogger[^2];

        foreach (var command in thisTurn)
        {
            if (command.Contains("erase:"))
            {
                var line = command.Split(" ");
                var name = line[1];
                var color = line[2];
                var status = line[3];

                if (color == "Black")
                    _blackFigures++;
                else
                    _whiteFigures++;

                _gameBoard.AddFigure(name.First(), name.Last() - '0',
                    color == "White" ? Color.White : Color.Black,
                    status == "Checker" ? Status.Checker : Status.Queen);

                returnCommandList.Add($"draw: {name} {color} {status}");
            }

            if (command.Contains("transform:"))
            {
                var line = command.Split(" ");
                var name = line[1];
                var color = line[2];

                _gameBoard.DeleteFigure(name.First(), name.Last() - '0');

                _gameBoard.AddFigure(name.First(), name.Last() - '0',
                    color == "White" ? Color.White : Color.Black, Status.Checker);

                returnCommandList.Add($"transformBack ${name} ${color}");
            }

            if (command.Contains("move:"))
            {
                var line = command.Split(" ");
                var nameFrom = line[1];
                var nameTo = line[2];
                var color = line[3];
                var status = line[4];

                _gameBoard.DeleteFigure(nameTo.First(), nameTo.Last() - '0');

                _gameBoard.AddFigure(nameFrom.First(), nameFrom.Last() - '0',
                    color == "White" ? Color.White : Color.Black, status == "Checker" ? Status.Checker : Status.Queen);

                returnCommandList.Add($"move: {nameTo} {nameFrom} {color} {status}");
                returnCommandList.Add("unselect");
            }

            if (command.Contains("change_color")) _turnColor = _turnColor == Color.White ? Color.Black : Color.White;
        }

        _turnLogger.RemoveAt(_turnLogger.Count - 2);
        _picked = null;
        _turnStatus = TurnStatus.WaitingFigurePick;

        returnCommandList.Add(DrawPossiblePickCommand());

        return returnCommandList;
    }

    private string DrawPossibleMoveCommand()
    {
        if (_picked == null)
            return "message: no figure picked";

        var possibleMoves = _gameBoard.WhereFigureCanMove(_picked.Item1, _picked.Item2);
        var command = "mark_cells: ";

        foreach (var line in possibleMoves)
            command += line.Item1 + line.Item2.ToString() + " ";

        return command[..^1];
    }

    private string DrawPossibleAttackCommand()
    {
        if (_picked == null)
            return "message: no figure picked";

        var possibleAttacks = _gameBoard.WhereFigureCanAttack(_picked.Item1, _picked.Item2);
        var command = "mark_cells: ";

        foreach (var line in possibleAttacks)
            command += line.Item1 + line.Item2.ToString() + " ";

        return command[..^1];
    }

    public string DrawPossiblePickCommand()
    {
        var possiblePicks = _gameBoard.AllPossibleToPick(_turnColor);
        var command = "mark_cells: ";

        foreach (var line in possiblePicks)
            command += line.Item1 + line.Item2.ToString() + " ";

        return command[..^1];
    }

    public List<string> Turn(char cellColumn, int cellRow)
    {
        if (cellColumn == 'u' && cellRow == 0)
            return UndoTurn();

        if (!GameContinues())
            return new List<string> {$"message: Game over, {(_whiteFigures == 0 ? "Black win" : "White win")}"};

        if (_turnStatus == TurnStatus.WaitingFigurePick)
        {
            var possiblePick = _gameBoard.AllPossibleToPick(_turnColor);

            if (!possiblePick.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                return new List<string>();

            _picked = new Tuple<char, int>(cellColumn, cellRow);
            _turnStatus = TurnStatus.WaitingCellToMovePick;

            var command = _gameBoard.WhereFigureCanAttack(_picked.Item1, _picked.Item2).Count > 0
                ? DrawPossibleAttackCommand()
                : DrawPossibleMoveCommand();

            return new List<string> {"unmark_cells", $"select_figure: {_picked.Item1}{_picked.Item2}", command};
        }
        else
        {
            if (new Tuple<char, int>(cellColumn, cellRow).Equals(_picked) && _turnStatus != TurnStatus.MultiAttack)
            {
                _picked = null;
                _turnStatus = TurnStatus.WaitingFigurePick;

                return new List<string> {"unselect", "unmark_cells", DrawPossiblePickCommand()};
            }

            if (_picked == null)
                return new List<string>();

            var transform = false;
            var possibleMoves = _gameBoard.WhereFigureCanMove(_picked.Item1, _picked.Item2);
            var possibleAttacks = _gameBoard.WhereFigureCanAttack(_picked.Item1, _picked.Item2);
            var returnCommandList = new List<string>();

            if (possibleAttacks.Count > 0)
            {
                if (!possibleAttacks.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                    return new List<string>();

                var killed = _gameBoard.GetKilledFiguresCell(
                    _picked.Item1, _picked.Item2, cellColumn, cellRow);

                var moveCommand = $"move: {_picked.Item1}{_picked.Item2} {cellColumn}{cellRow} {_turnColor} " +
                                  $"{_gameBoard.Cell(_picked.Item1, _picked.Item2)!.GetStatus()}";
                // this will get Null Ref Exc if it would be used after _gameBoard.MoveFigure or  _gameBoard.DeleteFigure

                var logCommand = $"log: {_picked.Item1}{_picked.Item2} {cellColumn}{cellRow} {_turnColor} " +
                                 $"{_gameBoard.Cell(_picked.Item1, _picked.Item2)!.GetStatus()}";
                // this will get Null Ref Exc if it would be used after _gameBoard.MoveFigure or  _gameBoard.DeleteFigure

                var killCommand = $"erase: {killed.Item1}{killed.Item2} " +
                                  $"{_gameBoard.Cell(killed.Item1, killed.Item2)!.GetColor()} " +
                                  $"{_gameBoard.Cell(killed.Item1, killed.Item2)!.GetStatus()}";
                // this will get Null Ref Exc if it would be used after  _gameBoard.MoveFigure or _gameBoard.DeleteFigure

                transform = _gameBoard.MoveFigure(_picked.Item1, _picked.Item2, cellColumn, cellRow);
                _gameBoard.DeleteFigure(killed.Item1, killed.Item2);

                _picked = new Tuple<char, int>(cellColumn, cellRow);

                if (_turnColor == Color.White)
                    _blackFigures--;
                else
                    _whiteFigures--;

                returnCommandList.Add("unmark_cells");
                returnCommandList.Add(moveCommand);
                returnCommandList.Add(logCommand);
                returnCommandList.Add(killCommand);

                if (transform) returnCommandList.Add($"transform: {_picked.Item1}{_picked.Item2} {_turnColor}");

                if (transform)
                    Log($"transform: {_picked.Item1}{_picked.Item2} {_turnColor}");
                Log(moveCommand);
                Log(killCommand);

                if (_gameBoard.WhereFigureCanAttack(_picked.Item1, _picked.Item2).Count == 0)
                {
                    NextTurn();

                    returnCommandList.Add("unselect");
                    returnCommandList.Add(DrawPossiblePickCommand());

                    Log("change_color");
                }
                else
                {
                    _turnStatus = TurnStatus.MultiAttack;

                    returnCommandList.Add(DrawPossibleAttackCommand());
                }

                Log("new_turn");
                return returnCommandList;
            }


            if (!possibleMoves.Contains(new Tuple<char, int>(cellColumn, cellRow)))
                return new List<string>();

            var command = $"move: {_picked.Item1}{_picked.Item2} {cellColumn}{cellRow} {_turnColor} " +
                          $"{_gameBoard.Cell(_picked.Item1, _picked.Item2)!.GetStatus()}";
            // this will get Null Ref Exc if it would be used after _gameBoard.MoveFigure

            var logCom = $"log: {_picked.Item1}{_picked.Item2} {cellColumn}{cellRow} {_turnColor} " +
                         $"{_gameBoard.Cell(_picked.Item1, _picked.Item2)!.GetStatus()}";
            // this will get Null Ref Exc if it would be used after _gameBoard.MoveFigure

            transform = _gameBoard.MoveFigure(_picked.Item1, _picked.Item2, cellColumn, cellRow);

            returnCommandList.Add("unmark_cells");
            returnCommandList.Add(command);
            returnCommandList.Add(logCom);
            returnCommandList.Add("unselect");

            if (transform) returnCommandList.Add($"transform: {cellColumn}{cellRow} {_turnColor}");

            if (transform)
                Log($"transform: {_picked.Item1}{_picked.Item2} {_turnColor}");
            Log(command);

            NextTurn();

            returnCommandList.Add(DrawPossiblePickCommand());

            Log("change_color");
            Log("new_turn");
            return returnCommandList;
        }
    }
}