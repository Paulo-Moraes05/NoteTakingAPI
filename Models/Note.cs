using System.ComponentModel.DataAnnotations;

namespace NoteTakingAPI.Models;

public class Note {
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Content { get; set; }

    public string? Summary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}