using System.ComponentModel.DataAnnotations;

namespace StoryGame.Models
{
    public class Story
    {
        public int StoryId { get; set; }

        [Required(ErrorMessage = "Story text is required.")]
        [StringLength(100, ErrorMessage = "Story text cannot exceed 100 characters.")]
        public string Text { get; set; } = string.Empty;

        public virtual List<Scene> ScenesList { get; set; } = new();
    }
}
