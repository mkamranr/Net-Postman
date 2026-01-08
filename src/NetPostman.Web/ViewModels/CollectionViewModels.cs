namespace NetPostman.Web.ViewModels;

/// <summary>
/// View models for collection management
/// </summary>
public class CollectionViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<RequestViewModel> Requests { get; set; } = new();
    public List<CollectionViewModel> SubCollections { get; set; } = new();
}

public class CreateCollectionViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid? ParentId { get; set; }
}

public class WorkspaceViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CollectionViewModel> Collections { get; set; } = new();
    public List<EnvironmentViewModel> Environments { get; set; } = new();
}

public class EnvironmentViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Variables { get; set; } = new();
    public bool IsGlobal { get; set; }
}

public class HistoryViewModel
{
    public Guid Id { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public string? StatusText { get; set; }
    public long? ResponseTime { get; set; }
    public DateTime ExecutedAt { get; set; }
}
