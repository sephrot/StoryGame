using Microsoft.AspNetCore.Mvc.Rendering;
using StoryGame.Models;

namespace StoryGame.ViewModels;

public class SceneViewModel
{
    public Scene Scene { get; set; } = default!;
    public bool HasFirstScene { get; set; }
    public bool HasThreeFinalScenes { get; set; }
    public bool HasChoices { get; set; }
}
