using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    // 🔑 Detect AJAX/fetch requests from the workspace
    private bool IsAjax() =>
        Request.Headers.TryGetValue("X-Requested-With", out var v) &&
        v.ToString().Contains("fetch");

    [HttpGet]
    public async Task<IActionResult> CreateScene(int storyId)
    {
        var story = await _storyDbContext
            .Stories.Include(s => s.ScenesList)
            .FirstOrDefaultAsync(s => s.StoryId == storyId);

        if (story == null)
            return NotFound("Story Not Found");

        bool hasFirstScene = story.ScenesList.Any(s => s.IsFirstScene);
        int countFinal = story.ScenesList.Count(s => s.IsFinalScene);
        bool hasThreeFinalScenes = countFinal >= 3;

        var sceneViewModel = new SceneViewModel
        {
            Scene = new Scene { StoryId = storyId },
            HasFirstScene = hasFirstScene,
            HasThreeFinalScenes = hasThreeFinalScenes,
        };

        // (Optional) You can make this return a partial in the future if needed
        return View(sceneViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateScene(Scene scene)
    {
        var story = await _storyDbContext.Stories.FindAsync(scene.StoryId);
        if (story == null)
            return NotFound("Story Not found");

        if (!ModelState.IsValid)
            return BadRequest();

        if (story.ScenesList == null)
            story.ScenesList = new List<Scene>();

        story.ScenesList.Add(scene);
        await _storyDbContext.SaveChangesAsync();

        // Keep legacy non-AJAX behavior
        if (IsAjax()) return Ok();
        return RedirectToAction("Create", "Story", new { id = scene.StoryId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var scene = await _storyDbContext
            .Scenes.Include(c => c.ChoiceList)
            .FirstOrDefaultAsync(s => s.SceneId == id);

        if (scene == null)
            return NotFound();

        bool hasChoices = scene.ChoiceList.Count > 0;
        var sceneViewModel = new SceneViewModel { Scene = scene, HasChoices = hasChoices };

        // 🔑 For workspace modal: render the same view as a partial
        if (IsAjax())
            return PartialView("Update", sceneViewModel);

        // Full-page edit still works as before
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

        // 🔑 For workspace modal: just return 200 so JS can refresh right panel
        if (IsAjax()) return Ok();

        // Keep your legacy flow for non-AJAX
        if (scene.ChoiceList == null || scene.ChoiceList.Count == 0)
        {
            return RedirectToAction("Create", "Choice", new { sceneId = scene.SceneId });
        }

        return RedirectToAction("Create", "Story", new { id = existingScene.StoryId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var scene = await _storyDbContext.Scenes.FindAsync(id);
        if (scene == null)
            return NotFound();

        // (Optional) Support partial delete dialog later if needed
        return View(scene);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var scene = await _storyDbContext
            .Scenes.Include(c => c.ChoiceList)
            .FirstOrDefaultAsync(s => s.SceneId == id);

        if (scene == null)
            return NotFound();

        var storyId = scene.StoryId;

        _storyDbContext.Choices.RemoveRange(scene.ChoiceList);
        _storyDbContext.Scenes.Remove(scene);
        await _storyDbContext.SaveChangesAsync();

        
        if (IsAjax()) return Ok();

        
        return RedirectToAction("Create", "Story", new { id = storyId });
    }
}
