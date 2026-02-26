using StudyFlow.Models;

namespace StudyFlow.Services;

public interface ISpacedRepetitionService
{
    List<Flashcard> GetDueFlashcards(StudyContent studyContent);
    Flashcard UpdateFlashcardSchedule(Flashcard flashcard, ReviewResult result);
    List<object> GenerateAdaptiveSession(StudyContent studyContent, int itemCount = 20);
}
