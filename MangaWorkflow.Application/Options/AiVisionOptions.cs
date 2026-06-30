namespace MangaWorkflow.Application.Options;

public class AiVisionOptions
{
    public string Mode { get; set; } = "Http";
    public string BaseUrl { get; set; } = "http://localhost:8001";
    public int TimeoutSeconds { get; set; } = 60;
    public bool EnableColorization { get; set; }
    public decimal AutoCreateRegionsMinConfidence { get; set; } = 0.60m;
    public bool RequireHumanApproval { get; set; } = true;
    public bool FallbackToMock { get; set; } = true;
}
