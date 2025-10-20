using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoryGame.DAL;
using StoryGame.Models;
using StoryGame.ViewModels;
namespace StoryGame.Controllers;

public class SceneController : Controller
{
    private readonly StoryDbContext _storyDbContext;

    public SceneController(StoryDbContext storyDbContext)
    {
        _storyDbContext = storyDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> CreateScene(int storyId)
    {
        var story = await _storyDbContext
            .Stories.Include(s => s.ScenesList)
            .FirstOrDefaultAsync(s => s.StoryId == storyId);
        if (story == null)
        {
            return NotFound("Story Not Found");
        }
        bool hasFirstScene = story.ScenesList.Any(s => s.IsFirstScene);
        bool hasThreeFinalScenes = false;
        int countFinal = story.ScenesList.Count(s => s.IsFinalScene);
        Console.WriteLine("Has total final: " + countFinal);
        if (countFinal >= 3)
        {
            hasThreeFinalScenes = true;
        }
        var sceneViewModel = new SceneViewModel
        {
            Scene = new Scene { StoryId = storyId },
            HasFirstScene = hasFirstScene,
            HasThreeFinalScenes = hasThreeFinalScenes,
        };
        return View(sceneViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateScene(Scene scene)
    {
        var story = await _storyDbContext.Stories.FindAsync(scene.StoryId); //find the story using storyId
        if (story == null)
            return NotFound("Story Not found");

        if (!ModelState.IsValid)
        {
            Console.WriteLine("Didnt work");
            return BadRequest();
        }

        if (story.ScenesList == null)
        {
            Console.WriteLine("New list created");
            story.ScenesList = new List<Scene>();
        }
        story.ScenesList.Add(scene);
        await _storyDbContext.SaveChangesAsync();
        Console.WriteLine("Worked");

        if (scene.IsFinalScene)
        {
            return RedirectToAction("Details", "Story", new { id = scene.StoryId });
        }
        
        return RedirectToAction("Update", "Scene", new { id = scene.SceneId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var scene = await _storyDbContext
            .Scenes.Include(c => c.ChoiceList)
            .FirstOrDefaultAsync(s => s.SceneId == id);
        bool hasChoices = true;

        if (scene == null)
        {
            return NotFound();
        }
        if (scene.ChoiceList.Count == 0)
        {
            hasChoices = false;
        }
        var sceneViewModel = new SceneViewModel { Scene = scene, HasChoices = hasChoices };
        return View(sceneViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Scene scene)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var existingScene = await _storyDbContext.Scenes.FindAsync(scene.SceneId);
        if (existingScene == null)
            return NotFound("Scene not found");

        existingScene.Text = scene.Text;
        existingScene.IsFinalScene = scene.IsFinalScene;

        await _storyDbContext.SaveChangesAsync();
        if (scene.ChoiceList.Count == 0)
        {
            return RedirectToAction("Create", "Choice", new { sceneId = scene.SceneId });
        }
        return RedirectToAction("TableStory", "Story");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var scene = await _storyDbContext.Scenes.FindAsync(id);
        if (scene == null)
        {
            return NotFound();
        }
        return View(scene);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var scene = await _storyDbContext
            .Scenes.Include(c => c.ChoiceList)
            .FirstOrDefaultAsync(s => s.SceneId == id);
        if (scene == null)
        {
            return NotFound();
        }
        _storyDbContext.Choices.RemoveRange(scene.ChoiceList);
        _storyDbContext.Scenes.Remove(scene);
        await _storyDbContext.SaveChangesAsync();
        return RedirectToAction("TableStory", "Story");
    }
}
