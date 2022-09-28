namespace Checkers.Models;

public enum Color
{
    White,
    Black
}

public enum Status
{
    Checker,
    Queen,
    Out
}

public class Figure
{
    private readonly Color _color;
    private Status _status;

    public Figure(Color color)
    {
        _color = color;
        _status = Status.Checker;
    }

    public Color GetColor()
    {
        return this._color;
    }

    public Status GetStatus()
    {
        return this._status;
    }

    public void SetStatus(Status status)
    {
        this._status = status;
    }
}