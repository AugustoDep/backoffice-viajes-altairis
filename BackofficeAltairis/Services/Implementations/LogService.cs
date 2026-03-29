using Microsoft.EntityFrameworkCore;
using BackofficeAltairis.Data;
using BackofficeAltairis.Models.Entities;
using BackofficeAltairis.Services.Interfaces;

namespace BackofficeAltairis.Services.Implementations;

public class LogService : ILogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LogService> _logger;

    public LogService(ApplicationDbContext context, ILogger<LogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(string action, string entity, int entityId, string entityName, string? details = null, string? user = null)
    {
        try
        {
            var log = new Log
            {
                Action = action,
                Entity = entity,
                EntityId = entityId,
                EntityName = entityName,
                Details = details,
                User = user ?? "System",
                CreatedAt = DateTime.Now
            };

            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create log");
        }
    }

    public async Task<List<Log>> GetAllLogsAsync()
    {
        return await _context.Logs
            .OrderByDescending(l => l.CreatedAt)
            .Take(500)
            .ToListAsync();
    }

    public async Task<List<Log>> GetLogsByDateAsync(DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);
        
        return await _context.Logs
            .Where(l => l.CreatedAt >= startDate && l.CreatedAt < endDate)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<DateTime, List<Log>>> GetLogsGroupedByDayAsync(int days = 30)
    {
        var startDate = DateTime.Now.AddDays(-days).Date;
        
        var logs = await _context.Logs
            .Where(l => l.CreatedAt >= startDate)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
        
        return logs
            .GroupBy(l => l.CreatedAt.Date)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(l => l.CreatedAt).ToList()
            );
    }
}