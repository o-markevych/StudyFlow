using System.Text;
using System.Text.RegularExpressions;
using StudyFlow.Models;

namespace StudyFlow.Services;

public class PdfProcessingService : IPdfProcessingService
{
    private readonly ILogger<PdfProcessingService> _logger;
    private const int MinChunkSize = 300;
    private const int MaxChunkSize = 800;

    public PdfProcessingService(ILogger<PdfProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExtractTextAsync(Stream pdfStream)
    {
        // TODO: Integrate a PDF library like iTextSharp, PdfPig, or Docnet.Core
        // For now, this is a placeholder that would need actual PDF extraction
        
        _logger.LogWarning("PDF text extraction not yet implemented. Using placeholder.");
        
        // Placeholder implementation
        await Task.Delay(100);
        
        throw new NotImplementedException(
            "PDF text extraction requires a PDF library. " +
            "Install one of: UglyToad.PdfPig, iText7, or Docnet.Core");
    }

    public async Task<List<DocumentChunk>> ChunkTextAsync(string text, Guid documentId)
    {
        var chunks = new List<DocumentChunk>();
        
        // Split by paragraphs and headings
        var sections = SplitIntoSections(text);
        
        int chunkIndex = 0;
        int currentPosition = 0;
        
        foreach (var section in sections)
        {
            if (string.IsNullOrWhiteSpace(section.Content))
                continue;
                
            // If section is too large, split it further
            if (section.Content.Length > MaxChunkSize)
            {
                var subChunks = SplitLargeSection(section.Content, MaxChunkSize);
                foreach (var subChunk in subChunks)
                {
                    chunks.Add(new DocumentChunk
                    {
                        DocumentId = documentId,
                        ChunkIndex = chunkIndex++,
                        Content = subChunk,
                        StartPosition = currentPosition,
                        EndPosition = currentPosition + subChunk.Length,
                        Heading = section.Heading,
                        PageNumber = 0 // TODO: Track page numbers during extraction
                    });
                    currentPosition += subChunk.Length;
                }
            }
            else if (section.Content.Length >= MinChunkSize)
            {
                chunks.Add(new DocumentChunk
                {
                    DocumentId = documentId,
                    ChunkIndex = chunkIndex++,
                    Content = section.Content,
                    StartPosition = currentPosition,
                    EndPosition = currentPosition + section.Content.Length,
                    Heading = section.Heading,
                    PageNumber = 0
                });
                currentPosition += section.Content.Length;
            }
        }
        
        _logger.LogInformation("Created {ChunkCount} chunks for document {DocumentId}", chunks.Count, documentId);
        return await Task.FromResult(chunks);
    }

    private List<Section> SplitIntoSections(string text)
    {
        var sections = new List<Section>();
        
        // Split by common heading patterns
        var headingPattern = @"^(#{1,6}\s+.+|[A-Z][^.!?]*[:]\s*$|\d+\.\s+[A-Z].+$)";
        var lines = text.Split('\n');
        
        var currentSection = new StringBuilder();
        string? currentHeading = null;
        
        foreach (var line in lines)
        {
            if (Regex.IsMatch(line.Trim(), headingPattern, RegexOptions.Multiline))
            {
                // Save previous section
                if (currentSection.Length > 0)
                {
                    sections.Add(new Section
                    {
                        Heading = currentHeading,
                        Content = currentSection.ToString().Trim()
                    });
                    currentSection.Clear();
                }
                
                currentHeading = line.Trim();
            }
            else
            {
                currentSection.AppendLine(line);
            }
        }
        
        // Add the last section
        if (currentSection.Length > 0)
        {
            sections.Add(new Section
            {
                Heading = currentHeading,
                Content = currentSection.ToString().Trim()
            });
        }
        
        return sections;
    }

    private List<string> SplitLargeSection(string text, int maxSize)
    {
        var chunks = new List<string>();
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+");
        
        var currentChunk = new StringBuilder();
        
        foreach (var sentence in sentences)
        {
            if (currentChunk.Length + sentence.Length > maxSize && currentChunk.Length > MinChunkSize)
            {
                chunks.Add(currentChunk.ToString().Trim());
                currentChunk.Clear();
            }
            
            currentChunk.Append(sentence).Append(" ");
        }
        
        if (currentChunk.Length > 0)
        {
            chunks.Add(currentChunk.ToString().Trim());
        }
        
        return chunks;
    }

    private class Section
    {
        public string? Heading { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
