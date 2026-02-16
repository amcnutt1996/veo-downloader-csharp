using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;

namespace VEOVideoDownloader.Services;

public class DialogService : IDialogService
{
    private readonly MessageBoxCustomParams _paramsError = new()
    {
        ButtonDefinitions = new List<ButtonDefinition>
        {
            new() { Name = "Ok" }
        },
        SystemDecorations = SystemDecorations.Full,
        ContentTitle = "Error", ContentMessage = "Please input a valid VEO URL.",
        WindowStartupLocation = WindowStartupLocation.CenterOwner,
        Icon = Icon.Error,
        CanResize = false,
        SizeToContent = SizeToContent.WidthAndHeight
    };

    private readonly Window _parentWindow;

    public DialogService(Window parentWindow)
    {
        _parentWindow = parentWindow ?? throw new ArgumentException(nameof(parentWindow));
    }


    public async Task ShowErrorAsync(string message)
    {
        try
        {
            var msgBox = MessageBoxManager.GetMessageBoxCustom(_paramsError);
            var result = await msgBox.ShowAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing messagebox: {ex.Message}");
        }
    }

    public async Task ShowInfoAsync(string message)
    {
        try
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Information",
                message,
                ButtonEnum.Ok,
                Icon.Info
            );
            await box.ShowWindowDialogAsync(_parentWindow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing info dialog: {ex.Message}");
        }
    }

    public async Task ShowWarningAsync(string message)
    {
        try
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Warning",
                message,
                ButtonEnum.Ok,
                Icon.Warning
            );
            await box.ShowWindowDialogAsync(_parentWindow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing warning dialog: {ex.Message}");
        }
    }

    public async Task<bool> ShowConfirmationAsync(string message)
    {
        try
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Confirm",
                message,
                ButtonEnum.YesNo,
                Icon.Question
            );
            var result = await box.ShowAsPopupAsync(_parentWindow);
            return result == ButtonResult.Yes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing confirmation dialog: {ex.Message}");
            return false;
        }
    }
}