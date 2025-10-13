using Microsoft.AspNetCore.Mvc.Rendering;
using StoryGame.Models;

namespace StoryGame.ViewModels;

public class ChoiceViewModel
{
    public Choice Choice { get; set; } = default!;
    public List<SelectListItem> SceneSelectList { get; set; } = default!;
}
