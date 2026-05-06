namespace AxaFrance.WebEngine.Mcp.Tools.Appium.Models;

/// <summary>A fixed-size slice of the Appium page source XML, enabling token-efficient screen inspection.</summary>
public sealed class PageSourceChunk
{
    /// <summary>The session this chunk belongs to.</summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>Zero-based index of this chunk.</summary>
    public int ChunkIndex { get; set; }

    /// <summary>Total number of chunks for the current screen source.</summary>
    public int TotalChunks { get; set; }

    /// <summary>Total XML source size in KB.</summary>
    public double TotalSizeKB { get; set; }

    /// <summary>Chunk size in KB used to compute TotalChunks.</summary>
    public int ChunkSizeKB { get; set; }

    /// <summary>The XML content slice for this chunk.</summary>
    public string Content { get; set; } = string.Empty;
}
