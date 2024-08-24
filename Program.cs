using System;
using System.Numerics;
using Gtk;
using Cairo;
using Gdk;
using gtk_test;
using gtk_test.utils;
using Rectangle = Gdk.Rectangle;
using Window = Gtk.Window;

class MainClass
{
    static MainWindow mainWindow;
    static List<WidgetWindow> widgets = new List<WidgetWindow>();

    static void Main(string[] args)
    {
        Application.Init();

        mainWindow = new MainWindow();
        mainWindow.Show();
    
        // Додайте логіку для створення нових нотаток
        Vector2Int size = new Vector2Int(400, 200);
        Vector2Int position = PositionSetter.CenterTop(size);
        BackgroundWidget background = new BackgroundWidget(size, position);
        ClockWidget clockWidget = new ClockWidget(size, position);
        clockWidget.AddBackground(background);
        widgets.Add(clockWidget);
        clockWidget.ShowAll();
        background.ShowAll();
        
        // ClockWidget clockWidget2 = new ClockWidget(new Vector2Int(300, 100), new Vector2Int(100, 200));
        // widgets.Add(clockWidget2);
        // clockWidget2.ShowAll();

        Application.Run();
    }

    static void CreateNewWidget()
    {
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


