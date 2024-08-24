using Cairo;
using Gdk;
using Gtk;
using gtk_test.utils;

namespace gtk_test;

public class WidgetWindow : Gtk.Window
{
    public Vector2Int Size { get; protected set; }
    public Vector2Int Position { get; protected set; }
    public BackgroundWidget Background { get; protected set; }
    
    private bool _isDragging = false;
    private int _offsetX;
    private int _offsetY;
    
    public WidgetWindow(Vector2Int size, Vector2Int position) : base(Gtk.WindowType.Toplevel)
    {
        Size = size;
        Position = PositionSetter.ConvertPosition(position);
        TypeHint = Gdk.WindowTypeHint.Utility;
        Decorated = false;
        AppPaintable = true; 
        Visual = Screen.RgbaVisual;
        // сірий напів прозорий колір
        SetSizeRequest(size.x, size.y);
        WindowPosition = WindowPosition.None;
        Move(Position.x, Position.y);
        
        
        AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask | (int)EventMask.PointerMotionMask);

        ButtonPressEvent += OnButtonPressEvent;
        ButtonReleaseEvent += OnButtonReleaseEvent;
        MotionNotifyEvent += OnMotionNotifyEvent;
    }
    
    public void AddBackground(BackgroundWidget background)
    {
        Background = background;
    }
    void OnButtonPressEvent(object o, ButtonPressEventArgs args)
    {
        if (args.Event.Button == 1) // Ліва кнопка миші
        {
            _isDragging = true;
            _offsetX = (int)args.Event.X;
            _offsetY = (int)args.Event.Y;
        }
    }

    void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
        if (args.Event.Button == 1) // Ліва кнопка миші
        {
            _isDragging = false;
        }
    }

    void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        if (_isDragging)
        {
            int x = (int)args.Event.XRoot - _offsetX;
            int y = (int)args.Event.YRoot - _offsetY;
            Move(x, y);
            if (Background != null)
            {
                Background.Move(x, y);
            }
        }
    }
}

public class BackgroundWidget : WidgetWindow
{
    private string _currentDir;
    private Gdk.Pixbuf _backgroundImage;
    
    
    public BackgroundWidget(Vector2Int size, Vector2Int position) : base(size, position)
    {
        TypeHint = Gdk.WindowTypeHint.Utility;
        Decorated = false;
        AppPaintable = true; 
        Visual = Screen.RgbaVisual;
        // сірий напів прозорий колір
        SetSizeRequest(size.x, size.y);
        WindowPosition = WindowPosition.None;
        
        _currentDir = System.IO.Directory.GetCurrentDirectory();
        _backgroundImage = new Gdk.Pixbuf(_currentDir+"/resources/clock_background-2.png");
        
        Drawn += OnDraw;
    }
    
    private void OnDraw(object o, DrawnArgs args)
    {
        Context cr = args.Cr;

        // Очищення фону (зробити його повністю прозорим)
        cr.SetSourceRGBA(0, 0, 0, 0);
        cr.Operator = Operator.Source;
        cr.Paint();

        // Малювання зображення
        Gdk.CairoHelper.SetSourcePixbuf(cr, _backgroundImage, 0, 0);
        cr.Paint();
        
        base.OnDrawn(cr);
    }
}