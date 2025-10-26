using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StoryGame.Models;

namespace StoryGame.DAL;

public class SceneRepository : ISceneRepository
{
    private readonly StoryDbContext _db;
    private readonly ILogger<SceneRepository> _logger;

    public SceneRepository(StoryDbContext db, ILogger<SceneRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Scene>?> GetAllScenes()
    {
        try
        {
            return await _db.Scenes.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[SceneRepository] scenes ToListAsync() failed when GetAllScenes(), error message: {e}",
                e.Message
            );
            return null;
        }
    }

    public async Task<Story?> GetAllScenesByStoryId(int storyId)
    {
        try
        {
            return await _db.Stories.FindAsync(storyId);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[SceneRepository] stories FindAsync() failed when GetAllScenesByStoryId(),for storyId {StoryId:0000} error message: {e}",
                storyId,
                e.Message
            );
            return null;
        }
    }

    public async Task<Scene?> GetSceneById(int id)
    {
        try
        {
            return await _db.Scenes.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[SceneRepository] Scenes FirstOrDefaultAsync() failed when GetSceneById(),for SceneId {SceneId:0000} error message: {e}",
                id,
                e.Message
            );
            return null;
        }
    }

    public async Task<bool> Create(Scene scene)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(
                "[SceneRepository] Scene creation failed for scene {@scene}, error message: {e}",
                scene,
                e.Message
            );
            return false;
        }
    }

    public async Task<bool> Update(Scene newScene)
    {
        try
        {
            var oldScene = await _db.Scenes.FindAsync(newScene.SceneId);
            if (oldScene == null)
                return false;

            oldScene.Text = newScene.Text;
            oldScene.IsFinalScene = newScene.IsFinalScene;

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[SceneRepository] story FindAsync(id) failed when updating the sceneId {sceneId:0000}, error message: {e}",
                newScene,
                e.Message
            );
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var scene = await _db.Scenes.FindAsync(id);
            if (scene == null)
            {
                return false;
            }
            _db.Choices.RemoveRange(scene.ChoiceList);
            _db.Scenes.Remove(scene);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[SceneRepository] scene deletion failed for sceneId {SceneId:0000}, error message: {e}",
                id,
                e.Message
            );
            return false;
        }
    }
}
