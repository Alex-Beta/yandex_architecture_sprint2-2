namespace sinemaabyss_events.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class UserEvent: Event
{
    [Required]
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [JsonPropertyName("action")]
    public string Action { get; set; } = null!;

    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}