using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace sinemaabyss_events.Models;

public class EventResponse
{
    [Required]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("partition")]
    public int Partition { get; set; }

    [Required]
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [Required]
    [JsonPropertyName("event")]
    public Event Event { get; set; } = new();
}