using StudyFlow.Models;

namespace StudyFlow.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmbeddingService> _logger;
    private readonly Dictionary<Guid, List<DocumentChunk>> _chunkStore = new();

    public EmbeddingService(IHttpClientFactory httpClientFactory, ILogger<EmbeddingService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("OpenAI");
        _logger = logger;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        // TODO: Integrate with Azure OpenAI embeddings endpoint
        // For now, return a mock embedding vector
        await Task.Delay(50);
        
        var random = new Random(text.GetHashCode());
        var embedding = new float[1536]; // Standard OpenAI embedding dimension
        
        for (int i = 0; i < embedding.Length; i++)
        {
            embedding[i] = (float)(random.NextDouble() * 2 - 1);
        }
        
        // Normalize the vector
        var magnitude = Math.Sqrt(embedding.Sum(x => x * x));
        for (int i = 0; i < embedding.Length; i++)
        {
            embedding[i] /= (float)magnitude;
        }
        
        return embedding;
    }

    public async Task<List<DocumentChunk>> GenerateChunkEmbeddingsAsync(List<DocumentChunk> chunks)
    {
        _logger.LogInformation("Generating embeddings for {ChunkCount} chunks", chunks.Count);
        
        foreach (var chunk in chunks)
        {
            chunk.Embedding = await GenerateEmbeddingAsync(chunk.Content);
        }
        
        // Store chunks for search
        if (chunks.Count > 0)
        {
            var documentId = chunks[0].DocumentId;
            _chunkStore[documentId] = chunks;
        }
        
        return chunks;
    }

    public async Task<List<DocumentChunk>> SearchSimilarChunksAsync(string query, Guid documentId, int topK = 5)
    {
        if (!_chunkStore.TryGetValue(documentId, out var chunks))
        {
            return new List<DocumentChunk>();
        }
        
        var queryEmbedding = await GenerateEmbeddingAsync(query);
        
        // Calculate cosine similarity
        var rankedChunks = chunks
            .Select(chunk => new
            {
                Chunk = chunk,
                Similarity = CosineSimilarity(queryEmbedding, chunk.Embedding!)
            })
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .Select(x => x.Chunk)
            .ToList();
        
        return rankedChunks;
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must have the same length");
        
        float dotProduct = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
        }
        
        return dotProduct; // Already normalized vectors
    }
}
