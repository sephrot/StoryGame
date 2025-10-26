using System.Net.Security;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StoryGame.Models;

namespace StoryGame.DAL;

public class ChoiceRepository : IChoiceRepository
{
    private readonly StoryDbContext _db;
    private readonly ILogger<ChoiceRepository> _logger;

    public ChoiceRepository(StoryDbContext db, ILogger<ChoiceRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Scene>?> GetAvailableScenesForChoice(int sceneId)
    {
        try
        {
            var currentScene = await _db.Scenes.FindAsync(sceneId);
            if (currentScene == null)
            {
                return null;
            }
            var usedSceneIds = await _db
                .Choices.Where(c => c.NextSceneId != null)
                .Select(c => c.NextSceneId)
                .ToListAsync();
            var scenes = await _db
                .Scenes.Where(s =>
                    s.StoryId == currentScene.StoryId
                    && s.SceneId != currentScene.SceneId
                    && !s.IsFirstScene
                    && !usedSceneIds.Contains(s.SceneId)
                )
                .ToListAsync();
            return scenes;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[ChoiceRepository] Choice FindAsync failed for sceneId {sceneId:0000}, error message: {e}",
                sceneId,
                e.Message
            );
            return null;
        }
    }

    public async Task<Choice?> GetChoiceById(int id)
    {
        try
        {
            return await _db.Choices.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[ChoiceRepository] Choices FindAsync() failed when GetChoiceById for choiceid {ChoiceId:0000}, error message: {e}",
                id,
                e.Message
            );
            return null;
        }
    }

    public async Task<bool> Create(Choice choice)
    {
        try
        {
            var scene = await _db.Scenes.FindAsync(choice.ThisSceneId);
            if (scene == null)
                return false;
            if (scene.ChoiceList == null)
                scene.ChoiceList = new List<Choice>();
            if (scene.ChoiceList.Count() > 4)
                return false;
            scene.ChoiceList.Add(choice);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[ChoiceRepository] Choice creation failed for choice {@choice}, error message: {e}",
                choice,
                e.Message
            );
            return false;
        }
    }

    public async Task<bool> Update(Choice choice)
    {
        try
        {
            var oldChoice = await _db.Choices.FindAsync(choice.ChoiceId);

            if (oldChoice == null)
                return false;

            oldChoice.Text = choice.Text;
            oldChoice.ThisSceneId = choice.ThisSceneId;
            oldChoice.NextSceneId = choice.NextSceneId;
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[ChoiceRepository] choice FindAsync(id) failed when updating the choiceId {ChoiceId:0000}, error message: {e}",
                choice,
                e.Message
            );
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        var choice = await _db.Choices.FindAsync(id);

        if (choice == null)
            return false;

        _db.Choices.Remove(choice);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Choice?> GetChoiceAsync(int id)
    {
        try
        {
            var choice = await _db.Choices.FindAsync(id);

            return choice;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "[ChoiceRepository] Choices FindAsync() failed when GetChoiceAsync() for choiceId {ChoiceId:0000}, error message: {e}",
                e.Message,
                id
            );
            return null;
        }
    }
}
