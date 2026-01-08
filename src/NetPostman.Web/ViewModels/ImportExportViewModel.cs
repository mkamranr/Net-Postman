namespace NetPostman.Web.ViewModels;

/// <summary>
/// View model for import/export operations
/// </summary>
public class ImportResultViewModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int CollectionsImported { get; set; }
    public int RequestsImported { get; set; }
}

public class ExportViewModel
{
    public Guid CollectionId { get; set; }
    public string Format { get; set; } = "postman_collection_v2.1";
}
