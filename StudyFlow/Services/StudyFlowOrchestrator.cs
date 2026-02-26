using Microsoft.AspNetCore.Components.Forms;
using StudyFlow.Models;

namespace StudyFlow.Services;

public class StudyFlowOrchestrator
{
    private readonly IDocumentService _documentService;
    private readonly IPdfProcessingService _pdfProcessingService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IStudyContentGenerator _studyContentGenerator;
    private readonly IWebEnrichmentService _webEnrichmentService;
    private readonly ILogger<StudyFlowOrchestrator> _logger;

    public StudyFlowOrchestrator(
        IDocumentService documentService,
        IPdfProcessingService pdfProcessingService,
        IEmbeddingService embeddingService,
        IStudyContentGenerator studyContentGenerator,
        IWebEnrichmentService webEnrichmentService,
        ILogger<StudyFlowOrchestrator> logger)
    {
        _documentService = documentService;
        _pdfProcessingService = pdfProcessingService;
        _embeddingService = embeddingService;
        _studyContentGenerator = studyContentGenerator;
        _webEnrichmentService = webEnrichmentService;
        _logger = logger;
    }

    public async Task<(Document Document, StudyContent StudyContent)> ProcessDocumentAsync(
        IBrowserFile file, 
        IProgress<ProcessingProgress>? progress = null)
    {
        progress?.Report(new ProcessingProgress("Uploading document...", 10));
        
        // Step 1: Upload and store document
        var document = await _documentService.UploadDocumentAsync(file);
        document.Status = DocumentStatus.Processing;
        
        try
        {
            progress?.Report(new ProcessingProgress("Extracting text from PDF...", 20));
            
            // Step 2: Extract text from PDF
            string extractedText;
            await using (var fileStream = File.OpenRead(document.StoragePath))
            {
                extractedText = await _pdfProcessingService.ExtractTextAsync(fileStream);
            }
            
            if (string.IsNullOrWhiteSpace(extractedText))
            {
                throw new InvalidOperationException("No text could be extracted from the PDF");
            }
            
            progress?.Report(new ProcessingProgress("Chunking document...", 30));
            document.Status = DocumentStatus.Chunking;
            
            // Step 3: Chunk the text
            var chunks = await _pdfProcessingService.ChunkTextAsync(extractedText, document.Id);
            document.Chunks = chunks;
            
            progress?.Report(new ProcessingProgress("Generating embeddings...", 40));
            document.Status = DocumentStatus.Embedding;
            
            // Step 4: Generate embeddings for chunks
            await _embeddingService.GenerateChunkEmbeddingsAsync(chunks);
            
            progress?.Report(new ProcessingProgress("Extracting key concepts...", 50));
            document.Status = DocumentStatus.GeneratingContent;
            
            // Step 5: Extract concepts and generate study content
            var concepts = await _studyContentGenerator.ExtractConceptsAsync(extractedText);
            
            progress?.Report(new ProcessingProgress("Generating flashcards...", 60));
            var flashcards = await _studyContentGenerator.GenerateFlashcardsAsync(concepts, extractedText);
            
            progress?.Report(new ProcessingProgress("Creating multiple choice questions...", 70));
            var mcqs = await _studyContentGenerator.GenerateMCQsAsync(concepts, extractedText);
            
            progress?.Report(new ProcessingProgress("Generating practice questions...", 80));
            var shortAnswers = await _studyContentGenerator.GenerateShortAnswerQuestionsAsync(concepts, extractedText);
            
            progress?.Report(new ProcessingProgress("Enriching with web knowledge...", 90));
            document.Status = DocumentStatus.Enriching;
            
            // Step 6: Web enrichment
            var searchQueries = await _webEnrichmentService.GenerateSearchQueriesAsync(concepts);
            var enrichedKnowledge = await _webEnrichmentService.EnrichKnowledgeAsync(searchQueries);
            
            // Step 7: Create final study content
            var studyContent = new StudyContent
            {
                DocumentId = document.Id,
                Concepts = concepts,
                Flashcards = flashcards,
                MultipleChoiceQuestions = mcqs,
                ShortAnswerQuestions = shortAnswers,
                EnrichedKnowledge = enrichedKnowledge
            };
            
            document.StudyContent = studyContent;
            document.Status = DocumentStatus.Completed;
            
            progress?.Report(new ProcessingProgress("Complete!", 100));
            
            _logger.LogInformation(
                "Successfully processed document {DocumentId}: {ConceptCount} concepts, {FlashcardCount} flashcards, {McqCount} MCQs",
                document.Id, concepts.Count, flashcards.Count, mcqs.Count);
            
            return (document, studyContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process document {DocumentId}", document.Id);
            document.Status = DocumentStatus.Failed;
            document.ErrorMessage = ex.Message;
            throw;
        }
    }
}

public record ProcessingProgress(string Message, int PercentComplete);
