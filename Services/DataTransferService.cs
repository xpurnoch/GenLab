using System.IO;
using System.Text.Json;
using BioLabManager.Models;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace BioLabManager.Services
{
    class DataTransferService : IDataTransferService, IShowMessage, IExportPrompt
    {
        public async Task ExportAsync()
        {
            var format = AskExportFormat("Click Yes for Excel, No for JSON.");
            if (format == ExportFormat.Cancel) 
                return;

            string folder = Path.Combine( Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!
                .Parent!.Parent!.FullName, "ExportedFiles");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var data = await LoadDataFromDatabaseAsync();
            if (data == null || data.Count == 0)
            {
                Show("No data to export.");
                return;
            }

            string filePath = Path.Combine(folder, format == ExportFormat.Excel ? "biolab_export.xlsx" : "biolab_export.json");
            try
            {
                if (format == ExportFormat.Excel)
                    await Task.Run(() => ExportToExcel(data, filePath));
                else
                    await ExportToJsonAsync(data, filePath);

                Show($"Exported to:\n{filePath}");
            }
            catch (Exception ex)
            {
                Show($"Export failed:\n{ex.Message}");
            }
        }

        public async Task ImportAsync()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Import data",
                Filter = "JSON Files|*.json|Excel Files|*.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            string ext = Path.GetExtension(dialog.FileName).ToLower();
            List<Dictionary<string, object>> importedData = null;

            try
            {
                importedData = ext switch
                {
                    ".json" => await ImportFromJsonAsync(dialog.FileName),
                    ".xlsx" => await Task.Run(() => ImportFromExcel(dialog.FileName)),
                    _ => throw new NotSupportedException("Unsupported file format.")
                };
            }
            catch (Exception ex)
            {
                Show($"Failed to import file:\n{ex.Message}");
                return;
            }

            if (importedData == null)
                return;

            if (!ValidateImportedData(importedData))
                return;

            try
            {
                await InsertDataToDatabaseAsync(importedData);
                WeakReferenceMessenger.Default.Send(new Messages.SamplesImportedMessage(true));
                Show("Import successful.");
            }
            catch (Exception ex)
            {
                Show($"Failed to insert data:\n{ex.Message}");
            }
        }


        private async Task<List<Dictionary<string, object>>> LoadDataFromDatabaseAsync()
        {
            var results = new List<Dictionary<string, object>>();
            await using var db = new BioLabDbContext();
            var samples = await db.Samples
                .Include(s => s.Lab)
                .Include(s => s.User)
                .Where(s => s.UserId == AuthService.CurrentUser.Id)
                .ToListAsync();

            foreach (var sample in samples)
            {
                results.Add(new Dictionary<string, object>
                {
                    ["Identifier"] = sample.Identifier,
                    ["SampleType"] = sample.SampleType,
                    ["Status"] = sample.Status,
                    ["StorageLocation"] = sample.StorageLocation,
                    ["CollectedAt"] = sample.CollectedAt.ToString("yyyy-MM-dd"),
                    ["Sequence"] = sample.Sequence,
                    ["Lab"] = sample.Lab?.Name ?? "Unknown",
                    ["User"] = sample.User?.Username ?? "Unknown"
                });
            }
            return results;
        }

        private async Task ExportToJsonAsync(List<Dictionary<string, object>> data, string filePath)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        private void ExportToExcel(List<Dictionary<string, object>> data, string filePath)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Export");
            var headers = data[0].Keys.ToList();

            for (int col = 0; col < headers.Count; col++)
                worksheet.Cell(1, col + 1).Value = headers[col];

            for (int row = 0; row < data.Count; row++)
            {
                for (int col = 0; col < headers.Count; col++)
                {
                    worksheet.Cell(row + 2, col + 1).Value = data[row][headers[col]]?.ToString();
                }
            }
            workbook.SaveAs(filePath);
        }

        private async Task<List<Dictionary<string, object>>> ImportFromJsonAsync(string path)
        {
            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
        }

        private List<Dictionary<string, object>> ImportFromExcel(string path)
        {
            var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().ToList();
            var headers = rows[0].Cells().Select(c => c.Value.ToString()).ToList();
            var data = new List<Dictionary<string, object>>();
            foreach (var row in rows.Skip(1))
            {
                var dict = new Dictionary<string, object>();
                for (int i = 0; i < headers.Count; i++)
                {
                    dict[headers[i]] = row.Cell(i + 1).Value.ToString();
                }
                data.Add(dict);
            }
            return data;
        }

        private async Task InsertDataToDatabaseAsync(List<Dictionary<string, object>> data)
        {
            await using var db = new BioLabDbContext();
            db.Samples.RemoveRange(db.Samples.Where(s => s.UserId == AuthService.CurrentUser.Id));
            await db.SaveChangesAsync();
            foreach (var row in data)
            {
                string labName = row.GetValueOrDefault("Lab")?.ToString()?.Trim();
                labName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labName?.ToLower() ?? "Unknown");

                var lab = await db.Labs.FirstOrDefaultAsync(l => l.Name == labName);
                if (lab == null)
                {
                    lab = new Lab { Name = labName };
                    db.Labs.Add(lab);
                    await db.SaveChangesAsync();
                }

                var sample = new Sample
                {
                    Identifier = row.GetValueOrDefault("Identifier")?.ToString(),
                    SampleType = row.GetValueOrDefault("SampleType")?.ToString(),
                    Status = row.GetValueOrDefault("Status")?.ToString(),
                    StorageLocation = row.GetValueOrDefault("StorageLocation")?.ToString(),
                    Sequence = row.GetValueOrDefault("Sequence")?.ToString() ?? "-",
                    CollectedAt = DateTime.TryParse(row.GetValueOrDefault("CollectedAt")?.ToString(), out var dt) ? dt : DateTime.Now,
                    UserId = AuthService.CurrentUser.Id,
                    LabId = lab.Id
                };

                db.Samples.Add(sample);
            }
            await db.SaveChangesAsync();
        }

        private bool ValidateImportedData(List<Dictionary<string, object>> data)
        {
            string[] requiredKeys = {
                "Identifier", "SampleType", "Status", "StorageLocation",
                "CollectedAt", "Sequence", "Lab", "User"
            };

            var validTypes = new[] { "DNA", "RNA", "Protein", "Other" };
            var validStatuses = new[] { "Active", "Not Active" };

            foreach (var row in data)
            {
                foreach (var key in requiredKeys)
                {
                    if (!row.ContainsKey(key) || string.IsNullOrWhiteSpace(row[key]?.ToString()))
                    {
                        Show($"Missing or empty field '{key}' in imported data.");
                        return false;
                    }
                }

                if (!row["Identifier"].ToString().StartsWith("SMP-"))
                {
                    Show("'Identifier' must start with 'SMP-'.");
                    return false;
                }

                if (!row["StorageLocation"].ToString().StartsWith("BOX-"))
                {
                    Show("'StorageLocation' must start with 'BOX-'.");
                    return false;
                }

                if (!DateTime.TryParse(row["CollectedAt"]?.ToString(), out _))
                {
                    Show("Invalid date format in 'CollectedAt'. Expected format: YYYY-MM-DD.");
                    return false;
                }

                if (!validTypes.Contains(row["SampleType"].ToString()))
                {
                    Show($"Invalid 'SampleType'. Must be one of: {string.Join(", ", validTypes)}");
                    return false;
                }

                if (!validStatuses.Contains(row["Status"].ToString()))
                {
                    Show($"Invalid 'Status'. Must be 'Active' or 'Not Active'.");
                    return false;
                }
            }
            return true;
        }

        public void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        public ExportFormat AskExportFormat(string message, string title = "Export Format")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            return result switch
            {
                MessageBoxResult.Yes => ExportFormat.Excel,
                MessageBoxResult.No => ExportFormat.Json,
                _ => ExportFormat.Cancel
            };
        }
    }
}
