using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryGame.DAL;
using StoryGame.Models;

namespace StoryGame.Controllers;

public class StoryController : Controller
{
    private readonly IStoryRepository _storyRepository;

    public StoryController(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository;
    }

    public async Task<IActionResult> TableStory()
    {
        var stories = await _storyRepository.GetEverything();
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
        if (!ModelState.IsValid)
        {
            return NotFound();
        }
        await _storyRepository.Create(story);
        Console.WriteLine("Added: Id:" + story.StoryId + " " + story.Text + " to database");
        return RedirectToAction("TableStory", "Story");
    }

    public async Task<IActionResult> Details(int id)
    {
        var story = await _storyRepository.GetStoryById(id);
        if (story == null)
            return NotFound();
        return View(story);
    }

    public async Task<IActionResult> StoryView()
    {
        var stories = await _storyRepository.GetAllStories();
        return View(stories);
    }

    [HttpGet]
    public async Task<IActionResult> Story(int id)
    {
        var story = await _storyRepository.GetStoryById(id);
        Scene currentScene;
        if (story == null)
        {
            Console.WriteLine("This Ran");
            return NotFound();
        }
        if (story.ScenesList == null)
        {
            story.ScenesList = new List<Scene>();
        }

        currentScene = story.ScenesList.First();
        Console.WriteLine("This Story Id ran, tried to get");
        return View(currentScene);
    }

    [HttpPost]
    public async Task<IActionResult> Story(int storyId, int nextScene, int thisScene)
    {
        var story = await _storyRepository.GetStoryById(storyId);
        Console.WriteLine($"Story: {storyId} nextScene: {nextScene} thisScene: {thisScene}");
        if (story == null)
        {
            Console.WriteLine("This Ran");
            return NotFound();
        }

        var scene = story.ScenesList.FirstOrDefault(s => s.SceneId == nextScene);
        Console.WriteLine($"Story: {storyId} nextScene: {nextScene} thisScene: {thisScene}");
        return View(scene);
    }
}
