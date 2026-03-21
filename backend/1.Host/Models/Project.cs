using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PortfolioAPI.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public required string Title { get; set; }

        [BsonElement("description")]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public required string Description { get; set; }

        [BsonElement("technologies")]
        public List<string> Technologies { get; set; } = new();

        [BsonElement("gitHubUrl")]
        [Url(ErrorMessage = "GitHub URL must be a valid URL")]
        public string? GitHubUrl { get; set; }

        [BsonElement("liveUrl")]
        [Url(ErrorMessage = "Live URL must be a valid URL")]
        public string? LiveUrl { get; set; }

        [BsonElement("imageUrl")]
        [Url(ErrorMessage = "Image URL must be a valid URL")]
        public string? ImageUrl { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
