using System.ComponentModel.DataAnnotations;

namespace StoryGame.Models;

public class Choice
{
    public int ChoiceId { get; set; }

    [Required(ErrorMessage = "Choice text is required.")]
    [StringLength(50, ErrorMessage = "Choice text cannot exceed 500 characters.")]
    public string Text { get; set; } = string.Empty;

    public virtual Scene? NextScene { get; set; }
    public int? NextSceneId { get; set; }
    public virtual Scene? ThisScene { get; set; } = default!;
    public int ThisSceneId { get; set; }
}
