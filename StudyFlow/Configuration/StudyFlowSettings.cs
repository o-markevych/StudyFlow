namespace StudyFlow.Configuration;

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://api.openai.com/v1/";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
    public string ChatModel { get; set; } = "gpt-4o-mini";
    public int MaxTokens { get; set; } = 4000;
}

public class AzureOpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
    public string EmbeddingDeploymentName { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "2024-02-15-preview";
}

public class StudyFlowSettings
{
    public OpenAISettings OpenAI { get; set; } = new();
    public AzureOpenAISettings AzureOpenAI { get; set; } = new();
    public bool UseAzureOpenAI { get; set; } = false;
    public int MaxChunkSize { get; set; } = 800;
    public int MinChunkSize { get; set; } = 300;
    public int DefaultSessionSize { get; set; } = 20;
}
