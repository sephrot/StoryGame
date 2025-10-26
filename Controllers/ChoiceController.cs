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
    private readonly ILogger<ChoiceController> _logger;

    public ChoiceController(
        ISceneRepository sceneRepository,
        IChoiceRepository choiceRepository,
        ILogger<ChoiceController> logger
    )
    {
        _sceneRepository = sceneRepository;
        _choiceRepository = choiceRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int sceneId)
    {
        var currentScene = await _sceneRepository.GetSceneById(sceneId);

        if (currentScene == null)
        {
            _logger.LogError(
                "[ChoiceController] Scene not found while executing _sceneRepository.GetSceneById() for id {SceneId:0000}",
                sceneId
            );
            return NotFound();
        }
        var scenes = await _choiceRepository.GetAvailableScenesForChoice(sceneId);
        if (scenes == null)
        {
            _logger.LogError(
                "[ChoiceController] Choice not found while executing _choiceRepository.GetAvailableScenesForChoice() for id {SceneId:0000}",
                sceneId
            );
            return NotFound();
        }
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
        if (ModelState.IsValid)
        {
            bool returnOk = await _choiceRepository.Create(choice);
            if (returnOk)
                return RedirectToAction("TableStory", "Story", new { id = choice.ThisSceneId });
        }
        _logger.LogWarning("[ChoiceController] Choice creation failed {@choice}", choice);
        return View(choice);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var choice = await _choiceRepository.GetChoiceById(id);
        if (choice == null)
        {
            _logger.LogError(
                "[ChoiceController] Choice not found while executing _choiceRepository.GetChoiceById() for id {ChoiceId:0000}",
                id
            );
            return NotFound();
        }
        return View(choice);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Choice choice)
    {
        if (ModelState.IsValid)
        {
            bool returnOk = await _choiceRepository.Update(choice);
            if (returnOk)
                return RedirectToAction("TableStory", "Story");
        }

        _logger.LogWarning("[ChoiceController] Choice update failed {@choice}", choice);
        return View(choice);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var choice = await _choiceRepository.GetChoiceById(id);
        if (choice == null)
        {
            _logger.LogError(
                "[ChoiceController] Choice not found for the choiceId {ChoiceId:0000}",
                id
            );
            return BadRequest("Choice not found for the ChoiceId");
        }
        return View(choice);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool returnOk = await _choiceRepository.Delete(id);
        if (!returnOk)
        {
            _logger.LogError(
                "[ChoiceController] Choice Deletion failed for the ChoiceId {ChoiceId:0000}",
                id
            );
        }
        return RedirectToAction("TableStory", "Story");
    }
}
