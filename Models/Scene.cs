using System.ComponentModel.DataAnnotations;

namespace StoryGame.Models
{
    public class Scene
    {
        public int SceneId { get; set; }

        [Required(ErrorMessage = "Scene text is required.")]
        [StringLength(1000, ErrorMessage = "Scene text cannot exceed 1000 characters.")]
        public string Text { get; set; } = string.Empty;
        
        public int StoryId { get; set; }
        public virtual Story? Story { get; set; }
        public bool IsFinalScene { get; set; } = false;
        public bool IsFirstScene { get; set; } = false;
        public virtual List<Choice> ChoiceList { get; set; } = new();
    }
}
