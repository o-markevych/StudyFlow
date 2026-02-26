using StudyFlow.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace StudyFlow.Services;

public interface IDocumentService
{
    Task<Document> UploadDocumentAsync(IBrowserFile file);
    Task<Document?> GetDocumentAsync(Guid id);
    Task<List<Document>> GetAllDocumentsAsync();
    Task DeleteDocumentAsync(Guid id);
}
