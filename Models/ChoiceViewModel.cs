using Microsoft.AspNetCore.Mvc.Rendering;
using StoryGame.Models;

public class ChoiceViewModel
{
    public int ChoiceId { get; set; }
    public string Text { get; set; } = string.Empty;

    // The scene that this choice belongs to
    public int ThisSceneId { get; set; }

    // The scene that this choice leads to
    public int NextSceneId { get; set; }

    // Lists for dropdowns
    public SelectList SceneOptions { get; set; } = default!;
}
