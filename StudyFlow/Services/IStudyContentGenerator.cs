using StudyFlow.Models;

namespace StudyFlow.Services;

public interface IStudyContentGenerator
{
    Task<List<Concept>> ExtractConceptsAsync(string text);
    Task<List<Flashcard>> GenerateFlashcardsAsync(List<Concept> concepts, string sourceText);
    Task<List<MultipleChoiceQuestion>> GenerateMCQsAsync(List<Concept> concepts, string sourceText);
    Task<List<ShortAnswerQuestion>> GenerateShortAnswerQuestionsAsync(List<Concept> concepts, string sourceText);
}
