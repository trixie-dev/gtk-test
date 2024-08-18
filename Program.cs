using System;
using System.Numerics;
using Gtk;
using Cairo;
using Gdk;
using gtk_test.utils;
using Rectangle = Gdk.Rectangle;
using Window = Gtk.Window;

class MainClass
{
    static MainWindow mainWindow;
    static List<NoteWindow> notes = new List<NoteWindow>();

    static void Main(string[] args)
    {
        Application.Init();

        mainWindow = new MainWindow();
        mainWindow.Show();

        // Додайте логіку для створення нових нотаток
        CreateNewNote();
        CreateNewNote();

        Application.Run();
    }

    static void CreateNewNote()
    {
        NoteWindow note = new NoteWindow();
        notes.Add(note);
        note.ShowAll();
    }
}

class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        this.SkipTaskbarHint = true;
        this.SkipPagerHint = true;
        this.Decorated = false;
        this.SetSizeRequest(1, 1);
        this.Move(-9999, -9999);
    }
}

class NoteWindow : Gtk.Window
{
    
    private Label timeLabel;
    private bool isDragging = false;
    private int offsetX;
    private int offsetY;
    public NoteWindow() : base(Gtk.WindowType.Toplevel)
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
}

class ClockWindow : Window
{
    private Label timeLabel;
    private bool isDragging = false;
    private int offsetX;
    private int offsetY;

    public ClockWindow() : base("Київський час")
    {
        // Налаштування вікна
        SetDefaultSize(600, 60);
        SetPosition(WindowPosition.Center);
        AppPaintable = true;
        Visual = Screen.RgbaVisual;
        // decoration none
        Decorated = false;

        // Підписуємося на події миші
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

        // Обробники подій
        DeleteEvent += delegate { Application.Quit(); };
    }

    private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
    {
        if (args.Event.Button == 1) // Ліва кнопка миші
        {
            isDragging = true;
            offsetX = (int)args.Event.X;
            offsetY = (int)args.Event.Y;
        }
    }

    private void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
        if (args.Event.Button == 1) // Ліва кнопка миші
        {
            isDragging = false;
        }
    }

    private void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        if (isDragging)
        {
            int x = (int)args.Event.XRoot - offsetX;
            int y = (int)args.Event.YRoot - offsetY;
            Move(x, y);
        }
    }

    private bool UpdateTime()
    {
        // Отримуємо поточний час у Києві
        DateTime kyivTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "FLE Standard Time");
        timeLabel.Text = kyivTime.ToString("HH:mm:ss");
        return true; // Продовжувати оновлення
    }
}
