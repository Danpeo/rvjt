using System;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using ReactiveUI;

namespace RVJT.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string? _inputText;

    public string? InputText
    {
        get => _inputText;
        set => this.RaiseAndSetIfChanged(ref _inputText, value);
    }

    private string? _outputText;

    public string? OutputText
    {
        get => _outputText;
        set => this.RaiseAndSetIfChanged(ref _outputText, value);
    }

    private string _currentTheme = null!;

    public string CurrentTheme
    {
        get => _currentTheme;
        set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
    }

    private bool _backgroundMode;

    public bool BackgroundMode
    {
        get => _backgroundMode;
        set
        {
            this.RaiseAndSetIfChanged(ref _backgroundMode, value);
            if (value)
                StartClipboardMonitor();
            else
                StopClipboardMonitor();
        }
    }

    private string? _lastClipboardText;

    private CancellationTokenSource? _clipboardMonitorCancellationTokenSource;

    public ReactiveCommand<Unit, Unit> RearrangeCommand { get; }

    public ReactiveCommand<Unit, Unit> CopyToClipboardCommand { get; }

    public ReactiveCommand<Unit, Unit> ClearInputCommand { get; }

    public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

    public ReactiveCommand<Unit, Unit> ToggleBackgroundModeCommand { get; }

    public MainWindowViewModel()
    {
        CurrentTheme = Application.Current?.RequestedThemeVariant == ThemeVariant.Dark ? "Dark" : "Light";
        RearrangeCommand = ReactiveCommand.Create(RearrangeText);
        CopyToClipboardCommand = ReactiveCommand.Create(CopyToClipboard);
        ClearInputCommand = ReactiveCommand.Create(ClearInput);
        ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
        ToggleBackgroundModeCommand = ReactiveCommand.Create(ToggleBackgroundMode);
    }

    private void StartClipboardMonitor()
    {
        _clipboardMonitorCancellationTokenSource?.Cancel(); 
        _clipboardMonitorCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = _clipboardMonitorCancellationTokenSource.Token;

        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var clipboardText = await Clipboard.Get().GetTextAsync();
                    if (!string.IsNullOrWhiteSpace(clipboardText) && clipboardText != _lastClipboardText)
                    {
                        _lastClipboardText = clipboardText;
                        InputText = clipboardText; 
                        RearrangeText();         
                    }
                }
                catch
                {
                    // ignore
                }

                await Task.Delay(500, cancellationToken); 
            }
        }, cancellationToken);
    }

    private void StopClipboardMonitor()
    {
        _clipboardMonitorCancellationTokenSource?.Cancel();
        _clipboardMonitorCancellationTokenSource = null;
        _lastClipboardText = null;
    }

    private void RearrangeText()
    {
        if (InputText != null)
        {
            OutputText = string.Join(" ", InputText
                .Split(['\n'], StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Reverse());

            CopyToClipboard();
        }
    }

    private void CopyToClipboard() => Clipboard.Get().SetTextAsync(OutputText);

    private void ClearInput()
    {
        InputText = string.Empty;
        OutputText = string.Empty;
    }

    private void ToggleTheme()
    {
        if (Application.Current == null) return;

        if (CurrentTheme == "Light")
        {
            CurrentTheme = "Dark";
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            CurrentTheme = "Light";
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }
    }

    private void ToggleBackgroundMode() => BackgroundMode = !BackgroundMode;
}