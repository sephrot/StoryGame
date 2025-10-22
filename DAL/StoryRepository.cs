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

        public async Task<IEnumerable<Story>> GetAllStories()
        {
            return await _db.Stories.ToListAsync();
        }

        public async Task<IEnumerable<Story>> GetEverything()
        {
            return await _db.Stories
                .Include(s => s.ScenesList)
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
            _db.Stories.Add(story);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Story story)
        {
            _db.Stories.Update(story);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var story = await _db.Stories
                .Include(s => s.ScenesList)
                .ThenInclude(c => c.ChoiceList)
                .FirstOrDefaultAsync(s => s.StoryId == id);

            if (story == null)
            {
                return false;
            }

            _db.Stories.Remove(story);
            await _db.SaveChangesAsync();
            return true;
        }

        // 🆕 Added method to match IStoryRepository
        public IEnumerable<Scene> GetScenesByStoryId(int storyId)
        {
            return _db.Scenes
                .Where(s => s.StoryId == storyId)
                .OrderBy(s => s.SceneId) // change this to your preferred sort property if needed
                .ToList();
        }
    }
}
