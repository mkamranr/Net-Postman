using Microsoft.AspNetCore.Mvc;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;
using NetPostman.Web.ViewModels;
using Newtonsoft.Json;

namespace NetPostman.Web.Controllers;

/// <summary>
/// Controller for collection management operations
/// </summary>
public class CollectionController : Controller
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly ILogger<CollectionController> _logger;

    public CollectionController(
        ICollectionRepository collectionRepository,
        IWorkspaceRepository workspaceRepository,
        ILogger<CollectionController> logger)
    {
        _collectionRepository = collectionRepository;
        _workspaceRepository = workspaceRepository;
        _logger = logger;
    }

    public async Task<IActionResult> GetCollections()
    {
        var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
        if (workspace == null)
        {
            return Json(new { success = false, message = "No workspace found" });
        }

        var collections = await _collectionRepository.GetByWorkspaceIdAsync(workspace.Id);
        var result = collections.Select(c => new CollectionViewModel
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
                RequestId = r.Id,
                Method = r.Method,
                Url = r.Url,
                Name = r.Name,
                BodyType = r.BodyType,
                Body = r.Body,
                CollectionId = c.Id
            }).ToList(),
            SubCollections = c.SubCollections.Select(sc => new CollectionViewModel
            {
                Id = sc.Id,
                Name = sc.Name,
                WorkspaceId = sc.WorkspaceId,
                ParentId = sc.ParentId,
                Requests = sc.Requests.Select(r => new RequestViewModel
                {
                    RequestId = r.Id,
                    Method = r.Method,
                    Url = r.Url,
                    Name = r.Name,
                    BodyType = r.BodyType,
                    Body = r.Body,
                    CollectionId = sc.Id
                }).ToList()
            }).ToList()
        });

        return Json(new { success = true, collections = result });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCollectionViewModel model)
    {
        try
        {
            var workspace = await _workspaceRepository.GetDefaultWorkspaceAsync();
            if (workspace == null)
            {
                return Json(new { success = false, message = "No workspace found" });
            }

            var collection = new Collection
            {
                Name = model.Name,
                Description = model.Description,
                WorkspaceId = workspace.Id,
                ParentId = model.ParentId
            };

            var result = await _collectionRepository.CreateAsync(collection);
            return Json(new { success = true, id = result.Id, name = result.Name });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating collection");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] CollectionViewModel model)
    {
        try
        {
            var collection = await _collectionRepository.GetByIdAsync(model.Id);
            if (collection == null)
            {
                return Json(new { success = false, message = "Collection not found" });
            }

            collection.Name = model.Name;
            collection.Description = model.Description;
            collection.UpdatedAt = DateTime.UtcNow;

            await _collectionRepository.UpdateAsync(collection);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating collection");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _collectionRepository.DeleteAsync(id);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting collection");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveRequest([FromBody] RequestViewModel model)
    {
        try
        {
            if (model.RequestId.HasValue && model.RequestId.Value != Guid.Empty)
            {
                // Update existing request
                var existingRequest = await _collectionRepository.GetRequestByIdAsync(model.RequestId.Value);
                if (existingRequest == null)
                {
                    return Json(new { success = false, message = "Request not found" });
                }

                existingRequest.Name = model.RequestName ?? "Untitled Request";
                existingRequest.Method = model.Method;
                existingRequest.Url = model.Url;
                existingRequest.BodyType = model.BodyType;
                existingRequest.BodyJson = model.Body;
                existingRequest.HeadersJson = JsonConvert.SerializeObject(model.Headers);
                existingRequest.QueryParamsJson = JsonConvert.SerializeObject(model.QueryParams);
                existingRequest.PreRequestScript = model.PreRequestScript;
                existingRequest.TestScript = model.TestScript;
                existingRequest.UpdatedAt = DateTime.UtcNow;

                await _collectionRepository.UpdateRequestAsync(existingRequest);
                return Json(new { success = true, id = existingRequest.Id });
            }
            else
            {
                // Create new request
                if (!model.CollectionId.HasValue)
                {
                    return Json(new { success = false, message = "Collection ID is required" });
                }

                var request = new Request
                {
                    Name = model.RequestName ?? "Untitled Request",
                    Method = model.Method,
                    Url = model.Url,
                    BodyType = model.BodyType,
                    BodyJson = model.Body,
                    HeadersJson = JsonConvert.SerializeObject(model.Headers),
                    QueryParamsJson = JsonConvert.SerializeObject(model.QueryParams),
                    PreRequestScript = model.PreRequestScript,
                    TestScript = model.TestScript
                };

                var result = await _collectionRepository.AddRequestAsync(model.CollectionId.Value, request);
                return Json(new { success = true, id = result.Id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving request");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRequest(Guid id)
    {
        try
        {
            var request = await _collectionRepository.GetRequestByIdAsync(id);
            if (request == null)
            {
                return Json(new { success = false, message = "Request not found" });
            }

            var model = new RequestViewModel
            {
                RequestId = request.Id,
                Name = request.Name,
                Method = request.Method,
                Url = request.Url,
                BodyType = request.BodyType,
                Body = request.BodyJson,
                CollectionId = request.CollectionId,
                PreRequestScript = request.PreRequestScript,
                TestScript = request.TestScript
            };

            // Parse headers
            if (!string.IsNullOrEmpty(request.HeadersJson))
            {
                try
                {
                    model.Headers = JsonConvert.DeserializeObject<List<KeyValueViewModel>>(request.HeadersJson) 
                                    ?? new List<KeyValueViewModel>();
                }
                catch { model.Headers = new List<KeyValueViewModel>(); }
            }

            // Parse query params
            if (!string.IsNullOrEmpty(request.QueryParamsJson))
            {
                try
                {
                    model.QueryParams = JsonConvert.DeserializeObject<List<KeyValueViewModel>>(request.QueryParamsJson)
                                       ?? new List<KeyValueViewModel>();
                }
                catch { model.QueryParams = new List<KeyValueViewModel>(); }
            }

            return Json(new { success = true, request = model });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting request");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRequest(Guid id)
    {
        try
        {
            var result = await _collectionRepository.DeleteRequestAsync(id);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting request");
            return Json(new { success = false, message = ex.Message });
        }
    }
}
