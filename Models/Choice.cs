using System.ComponentModel.DataAnnotations.Schema;

namespace StoryGame.Models;

public class Choice
{
    public int ChoiceId { get; set; }
    public string Text { get; set; } = string.Empty;
    public Scene? NextScene { get; set; }
    public int? NextSceneId { get; set; }
    public Scene? ThisScene { get; set; } = default!;
    public int ThisSceneId { get; set; }
}
