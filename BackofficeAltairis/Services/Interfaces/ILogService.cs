using BackofficeAltairis.Models.Entities;

namespace BackofficeAltairis.Services.Interfaces;

public interface ILogService
{
    Task LogAsync(string action, string entity, int entityId, string entityName, string? details = null, string? user = null);
    Task<List<Log>> GetAllLogsAsync();
    Task<List<Log>> GetLogsByDateAsync(DateTime date);
    Task<Dictionary<DateTime, List<Log>>> GetLogsGroupedByDayAsync(int days = 30);
}