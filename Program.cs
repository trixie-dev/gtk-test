using System;
using Gtk;
using Cairo;
using Gdk;
using Window = Gtk.Window;

class MainClass
{
    public static void Main(string[] args)
    {
        Application.Init();

        MainWindow win = new MainWindow();
        win.ShowAll();

        Application.Run();
    }
}

class MainWindow : Window
{
    private Label timeLabel;
    private bool isDragging = false;
    private int offsetX;
    private int offsetY;

    public MainWindow() : base("Київський час")
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

    private void OnDraw(object o, DrawnArgs args)
    {
        using (Context cr = args.Cr)
        {
            cr.SetSourceRGBA(0.1, 0.1, 0.1, 0.6); // Темно-сірий колір з прозорістю 60%
            cr.Operator = Operator.Source;
            cr.Paint();
        }
    }
}
