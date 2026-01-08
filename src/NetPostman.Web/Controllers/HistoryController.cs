using Microsoft.AspNetCore.Mvc;
using NetPostman.Core.Interfaces;
using NetPostman.Web.ViewModels;

namespace NetPostman.Web.Controllers;

/// <summary>
/// Controller for history management operations
/// </summary>
public class HistoryController : Controller
{
    private readonly IRequestHistoryRepository _historyRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(
        IRequestHistoryRepository historyRepository,
        IWorkspaceRepository workspaceRepository,
        ILogger<HistoryController> logger)
    {
        _historyRepository = historyRepository;
        _workspaceRepository = workspaceRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory(int limit = 50)
    {
        try
        {
            var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
            if (workspace == null)
            {
                return Json(new { success = false, message = "No workspace found" });
            }

            var history = await _historyRepository.GetByWorkspaceIdAsync(workspace.Id, limit);
            var result = history.Select(h => new HistoryViewModel
            {
                Id = h.Id,
                Method = h.Method,
                Url = h.Url,
                StatusCode = h.StatusCode,
                StatusText = h.StatusText,
                ResponseTime = h.ResponseTime,
                ExecutedAt = h.ExecutedAt
            });

            return Json(new { success = true, history = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting history");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ClearHistory()
    {
        try
        {
            var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
            if (workspace == null)
            {
                return Json(new { success = false, message = "No workspace found" });
            }

            var result = await _historyRepository.ClearHistoryAsync(workspace.Id);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing history");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEntry(Guid id)
    {
        try
        {
            var result = await _historyRepository.DeleteEntryAsync(id);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting history entry");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetHistoryEntry(Guid id)
    {
        try
        {
            var entry = await _historyRepository.GetByIdAsync(id);
            if (entry == null)
            {
                return Json(new { success = false, message = "History entry not found" });
            }

            return Json(new
            {
                success = true,
                entry = new
                {
                    id = entry.Id,
                    method = entry.Method,
                    url = entry.Url,
                    statusCode = entry.StatusCode,
                    statusText = entry.StatusText,
                    responseTime = entry.ResponseTime,
                    responseSize = entry.ResponseSize,
                    responseBody = entry.ResponseBody,
                    executedAt = entry.ExecutedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting history entry");
            return Json(new { success = false, message = ex.Message });
        }
    }
}
