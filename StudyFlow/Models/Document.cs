namespace StudyFlow.Models;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = "application/pdf";
    public string StoragePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;
    public string? ErrorMessage { get; set; }
    
    public List<DocumentChunk> Chunks { get; set; } = new();
    public StudyContent? StudyContent { get; set; }
}

public enum DocumentStatus
{
    Uploaded,
    Processing,
    Chunking,
    Embedding,
    GeneratingContent,
    Enriching,
    Completed,
    Failed
}
