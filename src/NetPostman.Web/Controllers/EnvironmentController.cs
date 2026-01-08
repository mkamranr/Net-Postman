using Microsoft.AspNetCore.Mvc;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;
using NetPostman.Web.ViewModels;
using Newtonsoft.Json;

namespace NetPostman.Web.Controllers;

/// <summary>
/// Controller for environment management operations
/// </summary>
public class EnvironmentController : Controller
{
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly ILogger<EnvironmentController> _logger;

    public EnvironmentController(
        IEnvironmentRepository environmentRepository,
        IWorkspaceRepository workspaceRepository,
        ILogger<EnvironmentController> logger)
    {
        _environmentRepository = environmentRepository;
        _workspaceRepository = workspaceRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetEnvironments()
    {
        try
        {
            var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
            if (workspace == null)
            {
                return Json(new { success = false, message = "No workspace found" });
            }

            var environments = await _environmentRepository.GetByWorkspaceIdAsync(workspace.Id);
            var result = environments.Select(e => new EnvironmentViewModel
            {
                Id = e.Id,
                Name = e.Name,
                IsGlobal = e.IsGlobal,
                Variables = ParseVariables(e.VariablesJson)
            });

            return Json(new { success = true, environments = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting environments");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EnvironmentViewModel model)
    {
        try
        {
            var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
            if (workspace == null)
            {
                return Json(new { success = false, message = "No workspace found" });
            }

            var environment = new Environment
            {
                WorkspaceId = workspace.Id,
                Name = model.Name,
                VariablesJson = JsonConvert.SerializeObject(model.Variables),
                IsGlobal = model.IsGlobal
            };

            var result = await _environmentRepository.CreateAsync(environment);
            return Json(new { success = true, id = result.Id, name = result.Name });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating environment");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] EnvironmentViewModel model)
    {
        try
        {
            var environment = await _environmentRepository.GetByIdAsync(model.Id);
            if (environment == null)
            {
                return Json(new { success = false, message = "Environment not found" });
            }

            environment.Name = model.Name;
            environment.VariablesJson = JsonConvert.SerializeObject(model.Variables);
            environment.UpdatedAt = DateTime.UtcNow;

            await _environmentRepository.UpdateAsync(environment);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating environment");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var environment = await _environmentRepository.GetByIdAsync(id);
            if (environment?.IsGlobal == true)
            {
                return Json(new { success = false, message = "Cannot delete global environment" });
            }

            var result = await _environmentRepository.DeleteAsync(id);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting environment");
            return Json(new { success = false, message = ex.Message });
        }
    }

    private Dictionary<string, string> ParseVariables(string json)
    {
        if (string.IsNullOrEmpty(json)) return new Dictionary<string, string>();
        
        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) 
                   ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
