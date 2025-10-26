using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoryGame.DAL;
using StoryGame.Models;

namespace StoryGame.Controllers;

public class StoryController : Controller
{
    private readonly IStoryRepository _storyRepository;
    private readonly ILogger<StoryController> _logger;

    public StoryController(IStoryRepository storyRepository, ILogger<StoryController> logger)
    {
        _storyRepository = storyRepository;
        _logger = logger;
    }

    public async Task<IActionResult> TableStory()
    {
        var stories = await _storyRepository.GetEverything();
        if (stories == null)
        {
            _logger.LogError(
                "[StoryController] Story list not found while executing _storyRepository.GetEverything()"
            );
            return NotFound("Story list not found");
        }
        foreach (var story in stories)
        {
            Console.WriteLine("StoryName: " + story.Text);
            if (story.ScenesList != null)
            {
                int length = story.ScenesList.Count;
                Console.WriteLine("Total Scenes: " + length);
            }
            else
            {
                Console.WriteLine("Total Scenes: 0");
            }
        }
        return View(stories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Story story)
    {
        if (ModelState.IsValid)
        {
            bool returnOk = await _storyRepository.Create(story);
            if (returnOk)
                return RedirectToAction("TableStory", "Story");
        }
        _logger.LogWarning("[StoryController] Story creation failed {@story}", story);
        return View(story);
    }

    public async Task<IActionResult> Details(int id)
    {
        var story = await _storyRepository.GetStoryById(id);
        if (story == null)
        {
            _logger.LogError(
                "[StoryController] Story list not found while executing _storyRepository.GetEverything()"
            );
            return NotFound();
        }
        return View(story);
    }

    public async Task<IActionResult> StoryView()
    {
        var stories = await _storyRepository.GetAllStories();
        if (stories == null)
        {
            _logger.LogError(
                "[StoryController] Story list not found while executing _storyRepository.GetAllStories()"
            );
            return NotFound("Story list not found");
        }
        return View(stories);
    }

    [HttpGet]
    public async Task<IActionResult> Story(int id)
    {
        var story = await _storyRepository.GetStoryById(id);
        Scene currentScene;
        if (story == null)
        {
            _logger.LogError(
                "[StoryController] Story not found while executing _storyRepository.GetStoryById() for id {StoryId:0000}",
                id
            );
            return NotFound("Story Not Found for the StoryId");
        }
        if (story.ScenesList == null)
        {
            story.ScenesList = new List<Scene>();
        }

        currentScene = story.ScenesList.First();
        return View(currentScene);
    }

    [HttpPost]
    public async Task<IActionResult> Story(int storyId, int nextScene, int thisScene)
    {
        var story = await _storyRepository.GetStoryById(storyId);
        if (story == null)
        {
            _logger.LogError(
                "[StoryController] Story not found while executing _storyRepository.GetStoryById() for id {StoryId:0000}",
                storyId
            );
            return NotFound("Story Not Found for the StoryId");
        }

        var scene = story.ScenesList.FirstOrDefault(s => s.SceneId == nextScene);
        return View(scene);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var story = await _storyRepository.GetStoryById(id);
        if (story == null)
        {
            _logger.LogError(
                "[StoryController] Story not found while executing _storyRepository.GetStoryById() for id {StoryId:0000}",
                id
            );
            return NotFound("Story Not Found for the StoryId");
        }

        return View(story);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Story story)
    {
        if (ModelState.IsValid)
        {
            bool returnOk = await _storyRepository.Update(story);
            if (returnOk)
                return RedirectToAction("TableStory", "Story");
        }
        _logger.LogWarning("[StoryController] Story Update failed {@story}", story);
        return View(story);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var story = await _storyRepository.GetStoryById(id);
        if (story == null)
        {
            _logger.LogError(
                "[StoryController] Story not found for the storyId {StoryId:0000}",
                id
            );
            return BadRequest("Story not found for the storyid");
        }
        return View(story);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool returnOk = await _storyRepository.Delete(id);
        if (!returnOk)
        {
            _logger.LogError(
                "[StoryController] Story deletion failed for the storyId {StoryId:0000}",
                id
            );
        }
        return RedirectToAction("TableStory", "Story");
    }
}
