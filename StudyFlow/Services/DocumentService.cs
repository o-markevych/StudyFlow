using Microsoft.AspNetCore.Components.Forms;
using StudyFlow.Models;

namespace StudyFlow.Services;

public class DocumentService : IDocumentService
{
    private readonly Dictionary<Guid, Document> _documents = new();
    private readonly string _storagePath;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IWebHostEnvironment env, ILogger<DocumentService> logger)
    {
        _storagePath = Path.Combine(env.ContentRootPath, "UploadedDocuments");
        _logger = logger;
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<Document> UploadDocumentAsync(IBrowserFile file)
    {
        var document = new Document
        {
            FileName = file.Name,
            FileSize = file.Size,
            ContentType = file.ContentType,
            Status = DocumentStatus.Uploaded
        };

        var filePath = Path.Combine(_storagePath, $"{document.Id}_{file.Name}");
        document.StoragePath = filePath;

        try
        {
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024).CopyToAsync(fileStream);
            
            _documents[document.Id] = document;
            _logger.LogInformation("Document {DocumentId} uploaded successfully", document.Id);
            
            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload document {DocumentId}", document.Id);
            document.Status = DocumentStatus.Failed;
            document.ErrorMessage = ex.Message;
            throw;
        }
    }

    public Task<Document?> GetDocumentAsync(Guid id)
    {
        _documents.TryGetValue(id, out var document);
        return Task.FromResult(document);
    }

    public Task<List<Document>> GetAllDocumentsAsync()
    {
        return Task.FromResult(_documents.Values.OrderByDescending(d => d.UploadedAt).ToList());
    }

    public Task DeleteDocumentAsync(Guid id)
    {
        if (_documents.TryGetValue(id, out var document))
        {
            if (File.Exists(document.StoragePath))
            {
                File.Delete(document.StoragePath);
            }
            _documents.Remove(id);
            _logger.LogInformation("Document {DocumentId} deleted", id);
        }
        return Task.CompletedTask;
    }
}
