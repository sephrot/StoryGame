using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryGame.DAL;
using StoryGame.Models;

namespace StoryGame.Controllers;

public class ChoiceController : Controller
{
    private readonly StoryDbContext _storyDbContext;

    public ChoiceController(StoryDbContext storyDbContext)
    {
        _storyDbContext = storyDbContext;
    }

    [HttpGet]
    public IActionResult Create(int SceneId)
    {
        var choice = new Choice { ThisSceneId = SceneId };
        return View(choice);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Choice choice)
    {
        var scene = await _storyDbContext.Scenes.FindAsync(choice.ThisSceneId);

        if (scene == null)
        {
            return NotFound("Scene Not found");
        }
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Didnt work for choice");
            return BadRequest();
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
        return RedirectToAction("TableStory", "Story");
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
            return BadRequest();
        }

        var existingChoice = await _storyDbContext.Choices.FindAsync(choice.ChoiceId);
        if (existingChoice == null)
        {
            return NotFound("Choice Not found");
        }
        existingChoice.Text = choice.Text;
        await _storyDbContext.SaveChangesAsync();

        return RedirectToAction("TableStory", "Story");
    }
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        return NotFound();
    }
}
