using StudyFlow.Models;

namespace StudyFlow.Services;

public interface IWebEnrichmentService
{
    Task<List<string>> GenerateSearchQueriesAsync(List<Concept> concepts);
    Task<List<EnrichedKnowledge>> EnrichKnowledgeAsync(List<string> searchQueries);
}
