using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoryGame.DAL;
using StoryGame.Models;
using StoryGame.ViewModels;

namespace StoryGame.Controllers;

public class SceneController : Controller
{
    private readonly ISceneRepository _sceneRepository;

    public SceneController(ISceneRepository sceneRepository)
    {
        _sceneRepository = sceneRepository;
    }

    [HttpGet]
    public async Task<IActionResult> CreateScene(int storyId)
    {
        var story = await _sceneRepository.GetAllScenesByStoryId(storyId);
        if (story == null)
            return NotFound("Story Not Found");

        bool hasFirstScene = story.ScenesList.Any(s => s.IsFirstScene);
        bool hasThreeFinalScenes = false;
        int countFinal = story.ScenesList.Count(s => s.IsFinalScene);

        Console.WriteLine("Has total final: " + countFinal);

        if (countFinal >= 3)
            hasThreeFinalScenes = true;

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
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Didnt work");
            return BadRequest();
        }

        var success = await _sceneRepository.Create(scene);

        if (!success)
        {
            return NotFound("Scene Not found");
        }

        if (scene.IsFinalScene)
        {
            return RedirectToAction("Details", "Story", new { id = scene.StoryId });
        }

        return RedirectToAction("Update", "Scene", new { id = scene.SceneId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var scene = await _sceneRepository.GetSceneById(id);
        bool hasChoices = true;

        if (scene == null)
            return NotFound();

        if (scene.ChoiceList.Count == 0)
            hasChoices = false;

        var sceneViewModel = new SceneViewModel { Scene = scene, HasChoices = hasChoices };
        return View(sceneViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(SceneViewModel model)
    {
        var newScene = model.Scene;

        if (!ModelState.IsValid)
            return BadRequest();

        var success = await _sceneRepository.Update(newScene);
        if (!success)
        {
            Console.WriteLine($"SceneId posted: {newScene.SceneId}");
            return NotFound("Scene Not found");
        }
        if (newScene.ChoiceList.Count == 0)
        {
            return RedirectToAction("Create", "Choice", new { sceneId = newScene.SceneId });
        }
        return RedirectToAction("TableStory", "Story");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var scene = await _sceneRepository.GetSceneById(id);

        return View(scene);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var scene = await _sceneRepository.GetSceneById(id);
        if (scene == null)
        {
            return NotFound();
        }
        await _sceneRepository.Delete(id);
        return RedirectToAction("TableStory", "Story");
    }
}
