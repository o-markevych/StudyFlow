namespace StudyFlow.Models;

public class StudySession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid DocumentId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public List<ReviewItem> ReviewItems { get; set; } = new();
    public SessionStatistics Statistics { get; set; } = new();
}

public class ReviewItem
{
    public Guid ItemId { get; set; }
    public ReviewItemType Type { get; set; }
    public DateTime ReviewedAt { get; set; }
    public ReviewResult Result { get; set; }
    public TimeSpan TimeSpent { get; set; }
}

public class SessionStatistics
{
    public int TotalItems { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public Dictionary<string, int> TopicBreakdown { get; set; } = new();
}

public enum ReviewItemType
{
    Flashcard,
    MultipleChoice,
    ShortAnswer
}

public enum ReviewResult
{
    Correct,
    Incorrect,
    Partial,
    Skipped
}
