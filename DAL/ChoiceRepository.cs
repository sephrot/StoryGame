using System.Net.Security;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StoryGame.Models;

namespace StoryGame.DAL;

public class ChoiceRepository : IChoiceRepository
{
    private readonly StoryDbContext _db;

    public ChoiceRepository(StoryDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Scene?>> GetAvailableScenesForChoice(int sceneId)
    {
        var currentScene = await _db.Scenes.FirstOrDefaultAsync(s => s.SceneId == sceneId);
        var usedSceneIds = await _db.Choices.Where(c => c.NextSceneId != null).Select(c => c.NextSceneId).ToListAsync();
        var scenes = await _db.Scenes.Where(s => s.StoryId == currentScene.StoryId && s.SceneId != currentScene.SceneId
                && !s.IsFirstScene
                && !usedSceneIds.Contains(s.SceneId)
            )
            .ToListAsync();
        return scenes;
    }

    public async Task<Choice?> GetChoiceById(int id)
    {
        return await _db.Choices.FindAsync(id);
    }

    public async Task<bool> Create(Choice choice)
    {
        var scene = await _db.Scenes.Include(c => c.ChoiceList).FirstOrDefaultAsync(s => s.SceneId == choice.ThisSceneId);
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

    public async Task<bool> Update(Choice choice)
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

    public async Task<bool> Delete(int id)
    {
        var choice = await _db
            .Choices.FindAsync(id);

        if (choice == null)
            return false;

        _db.Choices.Remove(choice);
        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<Choice?> GetChoiceAsync(int id)
    {
        var choice = await _db
        .Choices.Include(s => s.ThisScene)
        .Include(n => n.NextScene)
        .FirstOrDefaultAsync(c => c.ChoiceId == id);

        return choice;
    }
}
