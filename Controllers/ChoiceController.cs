using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoryGame.DAL;
using StoryGame.Models;
using StoryGame.ViewModels;

namespace StoryGame.Controllers;

public class ChoiceController : Controller
{
    private readonly StoryDbContext _storyDbContext;

    public ChoiceController(StoryDbContext storyDbContext)
    {
        _storyDbContext = storyDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int sceneId)
    {
        var currentScene = await _storyDbContext.Scenes.FirstOrDefaultAsync(s =>
            s.SceneId == sceneId
        );
        if (currentScene == null)
        {
            Console.WriteLine("Scene not found: " + sceneId);
            return NotFound();
        }
        var scenes = await _storyDbContext
            .Scenes.Where(s => s.StoryId == currentScene.StoryId)
            .ToListAsync();

        var choiceViewModel = new ChoiceViewModel
        {
            Choice = new Choice { ThisSceneId = currentScene.SceneId },
            SceneSelectList = scenes
                .Select(scene => new SelectListItem
                {
                    Value = scene.SceneId.ToString(),
                    Text = scene.Text,
                })
                .ToList(),
        };

        return View(choiceViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Choice choice)
    {
        var scene = await _storyDbContext.Scenes.FindAsync(choice.ThisSceneId);

        if (scene == null)
        {
            return NotFound($"Scene Not found IS HERE: {scene}");
        }
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState Invalid");
        }
        if (scene.ChoiceList == null)
        {
            Console.WriteLine("New ChoiceList Created");
            scene.ChoiceList = new List<Choice>();
        }
        if (scene.ChoiceList.Count() > 4)
        {
            return BadRequest("Can only have 4 choices");
        }
        scene.ChoiceList.Add(choice);
        await _storyDbContext.SaveChangesAsync();
        Console.WriteLine("Worked for Choice!");
        return RedirectToAction("TableStory", "Story", new { id = scene.SceneId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var choice = await _storyDbContext.Choices.FindAsync(id);
        if (choice == null)
        {
            return NotFound();
        }
        return View(choice);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Choice choice)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                $"Model State was not Valid: ChoiceID: {choice.ChoiceId} NextSceneId: {choice.NextSceneId} ThisSceneId: {choice.ThisSceneId} Text: {choice.Text}"
            );
        }

        var existingChoice = await _storyDbContext.Choices.FindAsync(choice.ChoiceId);

        if (existingChoice == null)
        {
            return NotFound("Choice Not found");
        }
        existingChoice.Text = choice.Text;
        existingChoice.ThisSceneId = choice.ThisSceneId;
        existingChoice.NextSceneId = choice.NextSceneId;
        await _storyDbContext.SaveChangesAsync();

        return RedirectToAction("TableStory", "Story");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var choice = await _storyDbContext
            .Choices.Include(s => s.ThisScene)
            .Include(n => n.NextScene)
            .FirstOrDefaultAsync(c => c.ChoiceId == id);
        if (choice == null)
        {
            return NotFound();
        }
        return View(choice);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var choice = await _storyDbContext.Choices.FindAsync(id);
        if (choice == null)
        {
            return NotFound();
        }
        _storyDbContext.Choices.Remove(choice);
        await _storyDbContext.SaveChangesAsync();
        return RedirectToAction("TableStory", "Story");
    }
}
