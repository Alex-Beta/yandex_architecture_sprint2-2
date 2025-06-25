namespace sinemaabyss_events.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Event
{
    [Required]
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [Required]
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [Required]
    [JsonPropertyName("payload")]
    public object Payload { get; set; } = null!;
}