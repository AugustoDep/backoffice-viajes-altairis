using Microsoft.AspNetCore.Mvc;
using BackofficeAltairis.Services.Interfaces;

namespace BackofficeAltairis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogService logService, ILogger<LogsController> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        try
        {
            var logs = await _logService.GetAllLogsAsync();
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting logs");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("grouped")]
    public async Task<IActionResult> GetLogsGrouped(int days = 30)
    {
        try
        {
            var groupedLogs = await _logService.GetLogsGroupedByDayAsync(days);
            return Ok(groupedLogs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grouped logs");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}