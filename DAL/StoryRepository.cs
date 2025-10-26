using Microsoft.EntityFrameworkCore;
using StoryGame.Models;

namespace StoryGame.DAL;

public class StoryRepository : IStoryRepository
{
    private readonly StoryDbContext _db;
    private readonly ILogger<StoryRepository> _logger;

    public StoryRepository(StoryDbContext db, ILogger<StoryRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Story>?> GetAllStories()
    {
        try
        {
            return await _db.Stories.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[StoryRepository] stories ToListAsync() failed when GetAllStories(), error message: {e}",
                e.Message
            );
            return null;
        }
    }

    public async Task<IEnumerable<Story>?> GetEverything()
    {
        try
        {
            return await _db.Stories.Include(s => s.ScenesList).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[StoryRepository] stories ToListAsync() failed when GetEverything(), error message: {e}",
                e.Message
            );
            return null;
        }
    }

    public async Task<Story?> GetStoryById(int id)
    {
        try
        {
            return await _db.Stories.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[StoryRepository] story FirstOrDefaultASync() failed when GetStoryById() for storyid {StoryId:0000}, error message: {e}",
                id,
                e.Message
            );
            return null;
        }
    }

    public async Task<bool> Create(Story story)
    {
        try
        {
            _db.Stories.Add(story);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[StoryRepository] story creation failed for story {@story}, error message: {e}",
                story,
                e.Message
            );
            return false;
        }
    }

    public async Task<bool> Update(Story story)
    {
        try
        {
            _db.Stories.Update(story);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[StoryRepository] story FindAsync(id) failed when updating the storyId {StoryId:0000}, error message: {e}",
                story,
                e.Message
            );
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var story = await _db
                .Stories.FindAsync(id);
            if (story == null)
            {
                return false;
            }
            _db.Stories.Remove(story);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[StoryRepository] story deletion failed for storyId {StoryId:0000}, error message: {e}",
                id,
                e.Message
            );
            return false;
        }
    }
}
