using System;

namespace StoryGame.Models
{
    public class Scene
    {
        public int SceneId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int StoryId { get; set; }
        public Story? Story { get; set; }
        public bool IsFinalScene { get; set; } = false;
        public List<Choice> ChoiceList { get; set; } = new();
    }
}
