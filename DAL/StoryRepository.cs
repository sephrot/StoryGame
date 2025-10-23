using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoryGame.Models;

namespace StoryGame.DAL
{
    public class StoryRepository : IStoryRepository
    {
        private readonly StoryDbContext _db;

        public StoryRepository(StoryDbContext db)
        {
            _db = db;
        }

        // Sidebar list – lightweight and fast
        public async Task<IEnumerable<Story>> GetAllStories()
        {
            // Order by Title (maps to Text via NotMapped) or fall back to Text if you prefer
            return await _db.Stories
                .AsNoTracking()
                .OrderBy(s => s.Text) // use .OrderBy(s => s.Title) if you added the NotMapped Title property
                .ToListAsync();
        }

        // Table view – include related data as needed
        public async Task<IEnumerable<Story>> GetEverything()
        {
            return await _db.Stories
                .Include(s => s.ScenesList)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Story?> GetStoryById(int id)
        {
            return await _db.Stories
                .Include(s => s.ScenesList)
                    .ThenInclude(scene => scene.ChoiceList)
                .FirstOrDefaultAsync(s => s.StoryId == id);
        }

        public async Task Create(Story story)
        {
            // No CreatedAt/UpdatedAt since your model doesn't have them
            await _db.Stories.AddAsync(story);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Story story)
        {
            // Safer than blind Update(): fetch and map only editable fields
            var existing = await _db.Stories.FirstOrDefaultAsync(s => s.StoryId == story.StoryId);
            if (existing == null) return;

            // If you added NotMapped Title that maps to Text, this updates Text under the hood
            existing.Text = story.Text;

            // If you later persist Summary/Content, add real columns and map them here
            // existing.Summary = story.Summary;
            // existing.Content = story.Content;

            await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var story = await _db.Stories
                .Include(s => s.ScenesList)
                    .ThenInclude(c => c.ChoiceList)
                .FirstOrDefaultAsync(s => s.StoryId == id);

            if (story == null)
                return false;

            _db.Stories.Remove(story);
            await _db.SaveChangesAsync();
            return true;
        }

        public IEnumerable<Scene> GetScenesByStoryId(int storyId)
        {
            return _db.Scenes
                .Where(s => s.StoryId == storyId)
                .OrderBy(s => s.SceneId)
                .AsNoTracking()
                .ToList();
        }
    }
}
