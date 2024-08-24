using Cairo;
using Gdk;
using Gtk;
using gtk_test.utils;
using Rectangle = Gdk.Rectangle;

namespace gtk_test;

public class ClockWidget: WidgetWindow
{
    
    private Label _timeLabel;
    private Label _dateLabel;
    private LevelBar levelBar;
    
    private string currentDir;
    
    
    public ClockWidget(Vector2Int size, Vector2Int position) : base(size, position)
    {
        currentDir = System.IO.Directory.GetCurrentDirectory();
        LoadCss();
        
        _timeLabel = new Label("00:00:00");
        _timeLabel.ModifyFont(Pango.FontDescription.FromString("Sans 48"));
        _timeLabel.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
        
        _dateLabel = new Label("Monday, 16 April 2024");
        _dateLabel.ModifyFont(Pango.FontDescription.FromString("Sans 18"));
        _dateLabel.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
        
        // Створення LevelBar для відображення прогресу доби
        levelBar = new LevelBar(0, 1)
        {
            Valign = Align.Start,
            MinValue = 0,
            MaxValue = 1,
            Value = 0,
            WidthRequest = 250,
            HeightRequest = 10,
        };
        
        levelBar.AddOffsetValue("zero", 0);
        levelBar.AddOffsetValue("six", 0.25);
        levelBar.AddOffsetValue("twelve", 0.5);
        levelBar.AddOffsetValue("eighteen", 0.75);
        levelBar.AddOffsetValue("twenty-four", 1);
        levelBar.StyleContext.AddClass("level-bar");
        
        HBox clockBox = new HBox();
        clockBox.PackStart(_timeLabel, expand: false, fill: false, padding: 10);
        clockBox.Halign = Align.Center;
        
        HBox dateBox = new HBox();
        dateBox.PackStart(_dateLabel, expand: false, fill: false, padding: 10);
        dateBox.Halign = Align.Center;
        
        HBox levelBarBox = new HBox();
        levelBarBox.PackStart(levelBar, expand: true, fill: true, padding: 10);
        levelBarBox.Halign = Align.Center;
        levelBarBox.WidthRequest = 280;
        
        
        VBox vbox = new VBox();
        vbox.PackStart(clockBox, expand: false, fill: true, padding: 6);
        vbox.PackStart(dateBox, expand: false, fill: true, padding: 6);
        vbox.PackStart(levelBarBox, expand: true, fill: true, padding: 6);
        Add(vbox);
        

        GLib.Timeout.Add(1000, UpdateTime);
    }
    
    bool UpdateTime()
    {
        DateTime kyivTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "FLE Standard Time");
        _timeLabel.Text = kyivTime.ToString("HH:mm:ss");
        
        // Оновлюємо дату
        _dateLabel.Text = kyivTime.ToString("dddd, d MMMM yyyy");
        
        //Обчислюємо прогрес доби
        double secondsInDay = kyivTime.TimeOfDay.TotalSeconds;
        double totalSecondsInDay = 24 * 60 * 60;
        levelBar.Value = secondsInDay / totalSecondsInDay;
        
        
        return true;
    }
    
    private void LoadCss()
    {
        // Завантаження CSS
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath(currentDir+"/resources/style.css"); // Шлях до вашого CSS файлу

        StyleContext.AddProviderForScreen(
            Gdk.Screen.Default,
            cssProvider,
            StyleProviderPriority.Application
        );
    }
}

public class Widget2Window : Gtk.Window
{
    protected Vector2Int Size;
    protected Vector2Int Position;
    
    private Label timeLabel;
    private bool isDragging = false;
    private int offsetX;
    private int offsetY;
    
    
    private Gdk.Pixbuf backgroundImage;
    
    public Widget2Window() : base(Gtk.WindowType.Toplevel)
    {
        
        Vector2Int size = new Vector2Int(300, 100);
        this.TypeHint = Gdk.WindowTypeHint.Utility;
        this.Decorated = false;
        AppPaintable = true;
        //Visual = Screen.RgbaVisual;
        this.SetSizeRequest(size.x, size.y);
        WindowPosition = WindowPosition.None;
        CenterTop(size);
        
        AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask | (int)EventMask.PointerMotionMask);

        ButtonPressEvent += OnButtonPressEvent;
        ButtonReleaseEvent += OnButtonReleaseEvent;
        MotionNotifyEvent += OnMotionNotifyEvent;
        
        // Створення мітки для відображення часу
        timeLabel = new Label("00:00:00");
        timeLabel.ModifyFont(Pango.FontDescription.FromString("Sans 48"));
        timeLabel.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));

        // Центрування мітки
        Alignment alignment = new Alignment(0.5f, 0.5f, 0f, 0f);
        alignment.Add(timeLabel);
        Add(alignment);

        // Оновлення часу кожну секунду
        GLib.Timeout.Add(1000, UpdateTime);
        
        //backgroundImage = new Gdk.Pixbuf(currentDir+"/resources/ClockBackground.png");
    }
    
    void CenterTop(Vector2Int size)
    {
        Screen screen = Screen.Default;
        int monitor = screen.GetMonitorAtWindow(screen.RootWindow);
        Rectangle monitorGeometry = screen.GetMonitorGeometry(monitor);

        // Розрахунок позиції по центру зверху
        int x = monitorGeometry.X + (monitorGeometry.Width - size.x) / 2;
        int y = monitorGeometry.Y;

        // Встановлення нової позиції вікна
        Move(x, y);
    }
    
    

    void OnButtonPressEvent(object o, ButtonPressEventArgs args)
    {
        if (args.Event.Button == 1) // Ліва кнопка миші
        {
            isDragging = true;
            offsetX = (int)args.Event.X;
            offsetY = (int)args.Event.Y;
        }
    }

    void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
        if (args.Event.Button == 1) // Ліва кнопка миші
        {
            isDragging = false;
        }
    }

    void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        if (isDragging)
        {
            int x = (int)args.Event.XRoot - offsetX;
            int y = (int)args.Event.YRoot - offsetY;
            Move(x, y);
        }
    }

    bool UpdateTime()
    {
        // Отримуємо поточний час у Києві
        DateTime kyivTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "FLE Standard Time");
        timeLabel.Text = kyivTime.ToString("HH:mm:ss");
        return true; // Продовжувати оновлення
    }
    
    
}