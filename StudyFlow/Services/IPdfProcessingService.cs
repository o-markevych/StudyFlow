using StudyFlow.Models;

namespace StudyFlow.Services;

public interface IPdfProcessingService
{
    Task<string> ExtractTextAsync(Stream pdfStream);
    Task<List<DocumentChunk>> ChunkTextAsync(string text, Guid documentId);
}
