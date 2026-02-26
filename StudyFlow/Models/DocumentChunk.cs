namespace StudyFlow.Models;

public class DocumentChunk
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public int PageNumber { get; set; }
    public string? Heading { get; set; }
    public float[]? Embedding { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}
