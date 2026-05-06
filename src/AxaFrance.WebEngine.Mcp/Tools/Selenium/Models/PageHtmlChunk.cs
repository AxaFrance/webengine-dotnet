namespace AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;

/// <summary>A fixed-size slice of the cleaned page HTML, enabling token-efficient page inspection.</summary>
public sealed class PageHtmlChunk
{
    /// <summary>The session this chunk belongs to.</summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>The page URL when the HTML was captured.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Zero-based index of this chunk.</summary>
    public int ChunkIndex { get; set; }

    /// <summary>Total number of chunks for the current page.</summary>
    public int TotalChunks { get; set; }

    /// <summary>Total cleaned HTML size in KB.</summary>
    public double TotalSizeKB { get; set; }

    /// <summary>Chunk size in KB used to compute TotalChunks.</summary>
    public int ChunkSizeKB { get; set; }

    /// <summary>The HTML content slice for this chunk.</summary>
    public string Content { get; set; } = string.Empty;
}
