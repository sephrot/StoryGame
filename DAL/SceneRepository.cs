using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StoryGame.Models;

namespace StoryGame.DAL;

public class SceneRepository : ISceneRepository
{
    private readonly StoryDbContext _db;

    public SceneRepository(StoryDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Scene?>> GetAllScenes()
    {
        return await _db.Scenes.ToListAsync();
    }

    public async Task<Story?> GetAllScenesByStoryId(int storyId)
    {
        return await _db
            .Stories.Include(s => s.ScenesList)
            .ThenInclude(c => c.ChoiceList)
            .FirstOrDefaultAsync(s => s.StoryId == storyId);
    }

    public async Task<Scene?> GetSceneById(int id)
    {
        return await _db
            .Scenes.Include(c => c.ChoiceList)
            .FirstOrDefaultAsync(s => s.SceneId == id);
    }

    public async Task<bool> Create(Scene scene)
    {
        var story = await _db.Stories.FindAsync(scene.StoryId); //find the story using storyId
        if (story == null)
            return false;

        if (story.ScenesList == null)
        {
            Console.WriteLine("New list created");
            story.ScenesList = new List<Scene>();
        }
        _db.Scenes.Add(scene);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Update(Scene newScene)
    {
        var oldScene = await _db.Scenes.FindAsync(newScene.SceneId);
        if (oldScene == null)
            return false;

        oldScene.Text = newScene.Text;
        oldScene.IsFinalScene = newScene.IsFinalScene;
        
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var scene = await _db
            .Scenes.Include(c => c.ChoiceList)
            .FirstOrDefaultAsync(s => s.SceneId == id);
        if (scene == null)
        {
            return false;
        }
        _db.Choices.RemoveRange(scene.ChoiceList);
        _db.Scenes.Remove(scene);
        await _db.SaveChangesAsync();
        return true;
    }
}
