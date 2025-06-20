namespace BioLabManager.Services
{
    public interface IExportPrompt
    {
        ExportFormat AskExportFormat(string message, string title = "Export Format");
    }
}
