namespace Checkers.Models;

public enum Color
{
    White,
    Black
}

public enum Status
{
    Checker,
    Queen
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
        return _color;
    }

    public Status GetStatus()
    {
        return _status;
    }

    public void SetStatus(Status status)
    {
        _status = status;
    }
}