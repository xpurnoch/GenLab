using BioLabManager.Models;
using BioLabManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BioLabManager.ViewModels;

public partial class BoxVisualizerViewModel : ObservableObject, IShowMessage
{
    [ObservableProperty] private ObservableCollection<BoxSummary> boxes = new();
    [ObservableProperty] private ObservableCollection<Sample> selectedBoxSamples = new();
    [ObservableProperty] private string selectedBoxName;

    [RelayCommand]
    public async Task LoadBoxesAsync()
    {
        try
        {
            await using var db = new BioLabDbContext();
            var grouped = await db.Samples
                .Where(s => s.UserId == AuthService.CurrentUser.Id)
                .GroupBy(s => s.StorageLocation)
                .Select(g => new BoxSummary
                {
                    BoxName = g.Key,
                    SampleCount = g.Count()
                })
                .OrderBy(b => b.BoxName)
                .ToListAsync();

            Boxes = new ObservableCollection<BoxSummary>(grouped);
        }
        catch (Exception ex)
        {
            Show($"Error while loading samples: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task LoadSamplesForBoxAsync(string boxName)
    {
        if (string.IsNullOrWhiteSpace(boxName))
        {
            Show("Name cannot be empty!");
            return;
        }

        try
        {
            await using var db = new BioLabDbContext();
            var samples = await db.Samples
                .Include(s => s.Lab)
                .Where(s => s.UserId == AuthService.CurrentUser.Id && s.StorageLocation == boxName)
                .ToListAsync();

            SelectedBoxSamples = new ObservableCollection<Sample>(samples);
            SelectedBoxName = boxName;
        }
        catch (Exception ex)
        {
            Show($"Error while loading samples: {ex.Message}");
        }
    }

    [RelayCommand]
    private void CopySequence(string? sequence)
    {
        if (string.IsNullOrWhiteSpace(sequence))
        {
            Show("Nothing to copy!");
            return;
        }

        try
        {
            Clipboard.SetText(sequence);
            Show("Sequence copied to clipboard!");
        }
        catch (Exception ex)
        {
            Show($"Cannot copy the sequence: {ex.Message}");
        }
    }

    public void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}
