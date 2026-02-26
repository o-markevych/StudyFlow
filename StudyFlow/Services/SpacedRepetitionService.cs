using StudyFlow.Models;

namespace StudyFlow.Services;

public class SpacedRepetitionService : ISpacedRepetitionService
{
    private readonly ILogger<SpacedRepetitionService> _logger;

    public SpacedRepetitionService(ILogger<SpacedRepetitionService> logger)
    {
        _logger = logger;
    }

    public List<Flashcard> GetDueFlashcards(StudyContent studyContent)
    {
        var now = DateTime.UtcNow;
        return studyContent.Flashcards
            .Where(f => f.NextReviewDate <= now)
            .OrderBy(f => f.NextReviewDate)
            .ToList();
    }

    public Flashcard UpdateFlashcardSchedule(Flashcard flashcard, ReviewResult result)
    {
        flashcard.RepetitionCount++;
        
        float quality = result switch
        {
            ReviewResult.Correct => 5.0f,
            ReviewResult.Partial => 3.0f,
            ReviewResult.Incorrect => 0.0f,
            ReviewResult.Skipped => 2.0f,
            _ => 0.0f
        };
        
        flashcard.EaseFactor = Math.Max(1.3f, 
            flashcard.EaseFactor + (0.1f - (5.0f - quality) * (0.08f + (5.0f - quality) * 0.02f)));
        
        if (quality < 3)
        {
            flashcard.RepetitionCount = 0;
            flashcard.IntervalDays = 1;
        }
        else
        {
            flashcard.IntervalDays = flashcard.RepetitionCount switch
            {
                1 => 1,
                2 => 6,
                _ => (int)Math.Round(flashcard.IntervalDays * flashcard.EaseFactor)
            };
        }
        
        flashcard.NextReviewDate = DateTime.UtcNow.AddDays(flashcard.IntervalDays);
        
        _logger.LogDebug("Updated flashcard {FlashcardId}: next review in {Days} days", 
            flashcard.Id, flashcard.IntervalDays);
        
        return flashcard;
    }

    public List<object> GenerateAdaptiveSession(StudyContent studyContent, int itemCount = 20)
    {
        var sessionItems = new List<object>();
        
        var dueFlashcards = GetDueFlashcards(studyContent);
        
        var flashcardQueue = new Queue<Flashcard>(dueFlashcards.Take(itemCount / 2));
        var mcqQueue = new Queue<MultipleChoiceQuestion>(
            studyContent.MultipleChoiceQuestions
                .OrderBy(_ => Random.Shared.Next())
                .Take(itemCount / 3));
        var shortAnswerQueue = new Queue<ShortAnswerQuestion>(
            studyContent.ShortAnswerQuestions
                .OrderBy(_ => Random.Shared.Next())
                .Take(itemCount / 6));
        
        while (sessionItems.Count < itemCount && 
               (flashcardQueue.Count > 0 || mcqQueue.Count > 0 || shortAnswerQueue.Count > 0))
        {
            if (flashcardQueue.Count > 0)
                sessionItems.Add(flashcardQueue.Dequeue());
                
            if (mcqQueue.Count > 0)
                sessionItems.Add(mcqQueue.Dequeue());
                
            if (shortAnswerQueue.Count > 0 && sessionItems.Count % 5 == 0)
                sessionItems.Add(shortAnswerQueue.Dequeue());
        }
        
        _logger.LogInformation("Generated adaptive session with {ItemCount} items", sessionItems.Count);
        return sessionItems;
    }
}
