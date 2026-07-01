namespace MangaWorkflow.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    // B9 FIX: HTTP status code passed in from UseStatusCodePagesWithReExecute
    public int? StatusCode { get; set; }
}
