using Microsoft.AspNetCore.Mvc;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;
using NetPostman.Web.ViewModels;
using Newtonsoft.Json;

namespace NetPostman.Web.Controllers;

/// <summary>
/// Main controller for the Postman-like application
/// </summary>
public class HomeController : Controller
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IRequestHistoryRepository _historyRepository;
    private readonly IHttpRequestService _httpRequestService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IWorkspaceRepository workspaceRepository,
        ICollectionRepository collectionRepository,
        IEnvironmentRepository environmentRepository,
        IRequestHistoryRepository historyRepository,
        IHttpRequestService httpRequestService,
        ILogger<HomeController> logger)
    {
        _workspaceRepository = workspaceRepository;
        _collectionRepository = collectionRepository;
        _environmentRepository = environmentRepository;
        _historyRepository = historyRepository;
        _httpRequestService = httpRequestService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
        if (workspace == null)
        {
            return View("Error", new { message = "No workspace found. Please ensure the database is initialized." });
        }

        var model = new WorkspaceViewModel
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Description = workspace.Description,
            IsActive = workspace.IsActive
        };

        // Load collections
        var collections = await _collectionRepository.GetByWorkspaceIdAsync(workspace.Id);
        model.Collections = collections.Select(c => new CollectionViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            WorkspaceId = c.WorkspaceId,
            ParentId = c.ParentId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Requests = c.Requests.Select(r => new RequestViewModel
            {
                Method = r.Method,
                Url = r.Url,
                Name = r.Name
            }).ToList()
        }).ToList();

        // Load environments
        var environments = await _environmentRepository.GetByWorkspaceIdAsync(workspace.Id);
        model.Environments = environments.Select(e => new EnvironmentViewModel
        {
            Id = e.Id,
            Name = e.Name,
            IsGlobal = e.IsGlobal,
            Variables = ParseVariables(e.VariablesJson)
        }).ToList();

        // Load history
        var history = await _historyRepository.GetByWorkspaceIdAsync(workspace.Id, 20);
        ViewBag.History = history.Select(h => new HistoryViewModel
        {
            Id = h.Id,
            Method = h.Method,
            Url = h.Url,
            StatusCode = h.StatusCode,
            StatusText = h.StatusText,
            ResponseTime = h.ResponseTime,
            ExecutedAt = h.ExecutedAt
        }).ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteRequest([FromBody] RequestViewModel request)
    {
        try
        {
            var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
            if (workspace == null)
            {
                return Json(new { success = false, message = "No workspace found" });
            }

            // Get environment variables
            var environments = await _environmentRepository.GetByWorkspaceIdAsync(workspace.Id);
            var variables = new Dictionary<string, string>();
            foreach (var env in environments)
            {
                foreach (var var in ParseVariables(env.VariablesJson))
                {
                    variables[var.Key] = var.Value;
                }
            }

            var executionRequest = new RequestExecutionRequest
            {
                Method = request.Method,
                Url = request.Url,
                Body = request.Body,
                BodyType = request.BodyType,
                Variables = variables,
                Headers = request.Headers.Select(h => new KeyValuePair
                {
                    Key = h.Key,
                    Value = h.Value,
                    Enabled = h.Enabled
                }).ToList()
            };

            var result = await _httpRequestService.ExecuteRequestAsync(executionRequest);

            // Save to history
            var history = new RequestHistory
            {
                WorkspaceId = workspace.Id,
                RequestId = request.RequestId,
                Method = request.Method,
                Url = request.Url,
                StatusCode = result.StatusCode,
                StatusText = result.StatusText,
                ResponseTime = result.ResponseTime,
                ResponseSize = result.ResponseSize,
                ResponseBody = result.Body?.Length > 10000 ? result.Body.Substring(0, 10000) : result.Body,
                ResponseHeadersJson = JsonConvert.SerializeObject(result.Headers),
                RequestBody = request.Body
            };
            await _historyRepository.AddAsync(history);

            return Json(new
            {
                success = true,
                statusCode = result.StatusCode,
                statusText = result.StatusText,
                responseTime = result.ResponseTime,
                responseSize = result.ResponseSize,
                body = result.Body,
                headers = result.Headers.Select(h => new KeyValueViewModel { Key = h.Key, Value = h.Value }),
                isSuccess = result.IsSuccess,
                errorMessage = result.ErrorMessage,
                historyId = history.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing request");
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
