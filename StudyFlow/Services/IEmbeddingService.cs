using StudyFlow.Models;

namespace StudyFlow.Services;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    Task<List<DocumentChunk>> GenerateChunkEmbeddingsAsync(List<DocumentChunk> chunks);
    Task<List<DocumentChunk>> SearchSimilarChunksAsync(string query, Guid documentId, int topK = 5);
}
