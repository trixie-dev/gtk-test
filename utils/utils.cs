using Gdk;

namespace gtk_test.utils;

public class Vector2Int(int x, int y)
{
    public int x { get; set; } = x;
    public int y { get; set; } = y;
}
public enum Position
{
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}


public static class PositionSetter
{
    /// <summary>
    /// Convert position returned by GetMonitorGeometry to the position that can be used in Move method
    /// </summary>
    public static Vector2Int ConvertPosition(Vector2Int position)
    {
        Screen screen = Screen.Default;
        int monitor = screen.GetMonitorAtWindow(screen.RootWindow);
        Rectangle monitorGeometry = screen.GetMonitorGeometry(monitor);
        
        return new Vector2Int(position.x + monitorGeometry.X, position.y + monitorGeometry.Y);
    }
    
    public static Vector2Int GetPosition(Position position, Vector2Int size)
    {
        Screen screen = Screen.Default;
        int monitor = screen.GetMonitorAtWindow(screen.RootWindow);
        Rectangle monitorGeometry = screen.GetMonitorGeometry(monitor);

        int x = 0, y = 0;

        switch (position)
        {
            case Position.TopLeft:
            case Position.CenterLeft:
            case Position.BottomLeft:
                x = monitorGeometry.X;
                break;
            case Position.TopCenter:
            case Position.Center:
            case Position.BottomCenter:
                x = monitorGeometry.X + (monitorGeometry.Width - size.x) / 2;
                break;
            case Position.TopRight:
            case Position.CenterRight:
            case Position.BottomRight:
                x = monitorGeometry.X + monitorGeometry.Width - size.x;
                break;
        }

        switch (position)
        {
            case Position.TopLeft:
            case Position.TopCenter:
            case Position.TopRight:
                y = monitorGeometry.Y;
                break;
            case Position.CenterLeft:
            case Position.Center:
            case Position.CenterRight:
                y = monitorGeometry.Y + (monitorGeometry.Height - size.y) / 2;
                break;
            case Position.BottomLeft:
            case Position.BottomCenter:
            case Position.BottomRight:
                y = monitorGeometry.Y + monitorGeometry.Height - size.y;
                break;
        }

        return new Vector2Int(x, y);
    }
    
    public static Vector2Int CenterTop(Vector2Int size)
    {
        Screen screen = Screen.Default;
        int monitor = screen.GetMonitorAtWindow(screen.RootWindow);
        Rectangle monitorGeometry = screen.GetMonitorGeometry(monitor);

        // Розрахунок позиції по центру зверху
        int x = monitorGeometry.X + (monitorGeometry.Width - size.x) / 2;
        int y = monitorGeometry.Y + 60;

        // Встановлення нової позиції вікна
        return new Vector2Int(x, y);

    }
}