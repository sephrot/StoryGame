using System;

namespace StoryGame.Models
{
    public class Story
    {
        public int StoryId { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<Scene> ScenesList { get; set; } = new();
    }
}
