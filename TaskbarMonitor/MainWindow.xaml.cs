using System;
using System.Windows;
using TaskbarMonitor.Models;

namespace TaskbarMonitor;

public partial class MainWindow : Window
{
    private AppConfig _config;

    public MainWindow()
    {
        InitializeComponent();
        _config = App.Config;
    }

    private System.Windows.Threading.DispatcherTimer _topmostTimer = null!;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        UpdatePosition();
        
        // Timer to enforce Topmost using UpdateZOrder (Lighter than property toggle)
        _topmostTimer = new System.Windows.Threading.DispatcherTimer();
        _topmostTimer.Interval = TimeSpan.FromSeconds(2); // Relax interval
        _topmostTimer.Tick += (s, args) => ReassertTopmost();
        _topmostTimer.Start();
    }

    private void ReassertTopmost()
    {
        var handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    private void UpdatePosition()
    {
        // Use System.Windows.Forms.Screen to get monitor info
        var screens = System.Windows.Forms.Screen.AllScreens;
        var targetIndex = _config.DisplaySettings.TargetDisplayIndex;
        
        if (targetIndex < 0 || targetIndex >= screens.Length)
        {
            targetIndex = 0; // Fallback to primary or 0
        }

        var screen = screens[targetIndex];
        var scaling = GetDpiScale();

        // Calculate logical position
        // WorkingArea excludes taskbar
        var workingArea = screen.WorkingArea;
        var bounds = screen.Bounds;

        // Determine Taskbar Position
        // If WorkingArea.Height < Bounds.Height -> Taskbar is visible
        // We want to place it next to system tray (usually bottom right)
        
        // Simple logic: Bottom Right of Working Area
        // Adjust for DPI
        
        double screenRight = workingArea.Right / scaling.DpiScaleX;
        double screenBottom = workingArea.Bottom / scaling.DpiScaleY;
        
        // Window dimensions (this.Width is Auto so we rely on ActualWidth after measure, but here we might need to update after rendering?)
        // Force measure
        this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var width = this.DesiredSize.Width;
        var height = this.DesiredSize.Height;

        // Position: 
        // Right margin: UiOffsetRight
        double rightMargin = _config.DisplaySettings.UiOffsetRight; // User config offset
        
        // Default System Tray Clock is roughly 100-150px wide depending on date format.
        // The user said "Left to the date display".
        // Windows 11 Taskbar date display width varies.
        // A manual offset is the safest verification-free way.
        
        this.Left = screenRight - width - rightMargin;
        this.Top = screenBottom - height; 

        // If taskbar is at bottom (standard), screenBottom usually aligns with top of taskbar.
        // But user wants it ON existing taskbar area? 
        // "Taskbar UI" -> "Left neighbor of Date Display".
        // The Prompt says: "タスクバーの日時表示の左隣にウィジェットとして表示する" (Display AS A WIDGET to the left of date display)
        // Usually this means it overlays the taskbar.
        // So we should use screen.Bounds.Bottom for Y reference, minus Taskbar Height? 
        // No, if it's NEXT to it, it should be ON the taskbar strip.
        // So Y should be inside the taskbar area.
        
        // Let's assume standard bottom taskbar.
        // WorkingArea.Bottom is the top edge of the taskbar.
        // Bounds.Bottom is the bottom edge of screen.
        
        double taskbarTop = workingArea.Bottom / scaling.DpiScaleY;
        double screenBottomLogical = bounds.Bottom / scaling.DpiScaleY;
        
        // Place it vertically centered in the taskbar? or just typically aligned.
        // Taskbar height = Bounds.Height - WorkingArea.Height (roughly)
        
        if (bounds.Bottom > workingArea.Bottom) // Taskbar at bottom
        {
            this.Top = taskbarTop + ((screenBottomLogical - taskbarTop) - height) / 2;
             // But wait, if we want to be ON the taskbar, we should use taskbarTop as top?
             // No, standard apps are occluded by taskbar if Topmost=false. We are Topmost=True.
             // We want to be visually inside the taskbar.
        }
        else
        {
            // Taskbar might be top/left/right or hidden.
            // Support simple case: Place at bottom right of working area for now if taskbar logic is complex.
            // But requirement is "Taskbar date display left neighbor".
            // Implementation: Place at bottom-right of SCREEN (minus offset).
            
            this.Top = screenBottomLogical - height; // Aligned to bottom of screen
        }
        
        // X adjustment
        // We start from Right edge of SCREEN.
        double screenRightLogical = bounds.Right / scaling.DpiScaleX;
        this.Left = screenRightLogical - width - rightMargin;
    }

    private DpiScale GetDpiScale()
    {
        var source = PresentationSource.FromVisual(this);
        if (source != null && source.CompositionTarget != null)
        {
            return new DpiScale(
                source.CompositionTarget.TransformToDevice.M11,
                source.CompositionTarget.TransformToDevice.M22);
        }
        return new DpiScale(1, 1);
    }

    private void ReloadPosition_Click(object sender, RoutedEventArgs e)
    {
        // Re-read config (optional, binding already does it for values, but for position we need manual trigger)
        // Here we just re-run positioning
        UpdatePosition();
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var vm = new ViewModels.SettingsViewModel(_config);
        var window = new Views.SettingsWindow(vm);
        window.ShowDialog();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}