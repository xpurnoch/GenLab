namespace BioLabManager.Services
{
    public interface IDataTransferService
    {
        Task ExportAsync();
        Task ImportAsync();
    }
}