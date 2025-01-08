using System;
using System.Linq;
using System.Reactive;
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

    private string _currentTheme;

    public string CurrentTheme
    {
        get => _currentTheme;
        set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
    }

    public ReactiveCommand<Unit, Unit> RearrangeCommand { get; }
    public ReactiveCommand<Unit, Unit> CopyToClipboardCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearInputCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

    public MainWindowViewModel()
    {
        CurrentTheme = Application.Current?.RequestedThemeVariant == ThemeVariant.Dark ? "Dark" : "Light";
        RearrangeCommand = ReactiveCommand.Create(RearrangeText);
        CopyToClipboardCommand = ReactiveCommand.Create(CopyToClipboard);
        ClearInputCommand = ReactiveCommand.Create(ClearInput);
        ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
    }

    private void RearrangeText()
    {
        if (InputText != null)
        {
            OutputText = string.Join(" ", InputText
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
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
}