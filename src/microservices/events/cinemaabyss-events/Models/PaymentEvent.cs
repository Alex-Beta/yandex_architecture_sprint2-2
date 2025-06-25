namespace sinemaabyss_events.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class PaymentEvent: Event
{
    [Required]
    [JsonPropertyName("payment_id")]
    public int PaymentId { get; set; }

    [Required]
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [Required]
    [JsonPropertyName("amount")]
    public float Amount { get; set; }

    [Required]
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("method_type")]
    public string? MethodType { get; set; }
}
