using System.Collections.Generic;

namespace StoryGame.Models.ViewModels
{
    public class WorkspaceViewModel
    {
        public IEnumerable<Story> Stories { get; set; } = new List<Story>();
        public Story? Selected { get; set; }
    }
}
