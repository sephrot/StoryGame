using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryGame.Models
{
    public class Story
    {
        public int StoryId { get; set; }

        // Your original persisted text field
        public string Text { get; set; } = string.Empty;

        public List<Scene> ScenesList { get; set; } = new();

        // --- View-facing properties (no DB change needed) ---

        /// <summary>
        /// Map Title to the existing Text column so views can use "Title".
        /// </summary>
        [NotMapped]
        [Required, StringLength(120)]
        public string Title
        {
            get => Text;
            set => Text = value ?? string.Empty;
        }

        /// <summary>
        /// Optional fields used by the editor UI.
        /// They are NotMapped so EF won't expect DB columns.
        /// </summary>
        [NotMapped] public string? Summary { get; set; }
        [NotMapped] public string? Content { get; set; }
    }
}
