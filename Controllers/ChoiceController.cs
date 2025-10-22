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
    private readonly ISceneRepository _sceneRepository;
    private readonly IChoiceRepository _choiceRepository;

    public ChoiceController(ISceneRepository sceneRepository, IChoiceRepository choiceRepository)
    {
        _sceneRepository = sceneRepository;
        _choiceRepository = choiceRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int sceneId)
    {
        var currentScene = await _sceneRepository.GetSceneById(sceneId);

        if (currentScene == null)
        {
            Console.WriteLine("Scene not found: " + sceneId);
            return NotFound();
        }
        var scenes = await _choiceRepository.GetAvailableScenesForChoice(sceneId);

        var choiceViewModel = new ChoiceViewModel
        {
            Choice = new Choice { ThisSceneId = currentScene.SceneId },
            SceneSelectList = scenes
                .Select(scene => new SelectListItem
                {
                    Value = scene?.SceneId.ToString(),
                    Text = scene?.Text,
                })
                .ToList(),
        };

        return View(choiceViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Choice choice)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState Invalid");
        }

        var success = await _choiceRepository.Create(choice);

        if (!success)
        {
            return NotFound("Choice Not found");
        }
        return RedirectToAction("TableStory", "Story", new { id = choice.ThisSceneId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var choice = await _choiceRepository.GetChoiceById(id);
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

        await _choiceRepository.Update(choice);
        return RedirectToAction("TableStory", "Story");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var choice = await _choiceRepository.GetChoiceById(id);
        if (choice == null)
        {
            return NotFound();
        }
        return View(choice);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var choice = await _choiceRepository.GetChoiceById(id);
        if (choice == null)
        {
            return NotFound("Choice Not found");
        }
        await _choiceRepository.Delete(id);
        return RedirectToAction("TableStory", "Story");
    }
}
