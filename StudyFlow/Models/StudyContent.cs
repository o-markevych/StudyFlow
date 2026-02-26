namespace StudyFlow.Models;

public class StudyContent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Concept> Concepts { get; set; } = new();
    public List<Flashcard> Flashcards { get; set; } = new();
    public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; } = new();
    public List<ShortAnswerQuestion> ShortAnswerQuestions { get; set; } = new();
    public List<EnrichedKnowledge> EnrichedKnowledge { get; set; } = new();
}

public class Concept
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public List<string> RelatedConcepts { get; set; } = new();
    public List<string> CommonMisconceptions { get; set; } = new();
    public string? Category { get; set; }
}

public class Flashcard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
    
    // Spaced repetition data
    public DateTime NextReviewDate { get; set; } = DateTime.UtcNow;
    public int RepetitionCount { get; set; } = 0;
    public float EaseFactor { get; set; } = 2.5f;
    public int IntervalDays { get; set; } = 1;
}

public class MultipleChoiceQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Question { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int CorrectOptionIndex { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
}

public class ShortAnswerQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Question { get; set; } = string.Empty;
    public string ModelAnswer { get; set; } = string.Empty;
    public List<string> KeyPoints { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
}

public class EnrichedKnowledge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Topic { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<Citation> Citations { get; set; } = new();
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}

public class Citation
{
    public string Source { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
}

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}
