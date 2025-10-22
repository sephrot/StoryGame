using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoryGame.DAL;
using StoryGame.Models;

namespace StoryGame.Controllers
{
    public class StoryController : Controller
    {
        private readonly IStoryRepository _storyRepository;

        public StoryController(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }

        // ---------------- Table (landing with buttons) ----------------
        public async Task<IActionResult> TableStory()
        {
            var stories = await _storyRepository.GetEverything();

            // Debug logs (optional)
            foreach (var story in stories)
            {
                Console.WriteLine("StoryName: " + story.Text);
                Console.WriteLine("Total Scenes: " + (story.ScenesList?.Count ?? 0));
            }

            return View(stories);
        }

        // ---------------- Create ----------------
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Story story)
        {
            if (!ModelState.IsValid)
            {
                // Return the same view to show validation errors instead of 404
                return View(story);
            }

            await _storyRepository.Create(story);
            Console.WriteLine($"Added: Id:{story.StoryId} {story.Text} to database");
            return RedirectToAction(nameof(TableStory));
        }

        // ---------------- Details (right work area) ----------------
        public async Task<IActionResult> Details(int id)
        {
            var story = await _storyRepository.GetStoryById(id);
            if (story == null)
                return NotFound();

            // Provide data for the LEFT sidebar and highlight the active story
            ViewBag.AllStories = await _storyRepository.GetAllStories();
            ViewBag.CurrentStoryId = id;

            return View(story);
        }

        // ---------------- Simple list view (optional) ----------------
        public async Task<IActionResult> StoryView()
        {
            var stories = await _storyRepository.GetAllStories();
            return View(stories);
        }

        // ---------------- Play/Read story flow (optional) ----------------
        [HttpGet]
        public async Task<IActionResult> Story(int id)
        {
            var story = await _storyRepository.GetStoryById(id);
            if (story == null)
                return NotFound();

            var currentScene = story.ScenesList?.FirstOrDefault();
            if (currentScene == null)
            {
                // No scenes yet—redirect to create scene or show a friendly view
                TempData["Info"] = "This story has no scenes yet.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(currentScene);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Story(int storyId, int nextScene, int thisScene)
        {
            var story = await _storyRepository.GetStoryById(storyId);
            Console.WriteLine($"Story: {storyId} nextScene: {nextScene} thisScene: {thisScene}");

            if (story == null)
                return NotFound();

            var scene = story.ScenesList?.FirstOrDefault(s => s.SceneId == nextScene);
            if (scene == null)
            {
                TempData["Info"] = "Next scene not found.";
                return RedirectToAction(nameof(Details), new { id = storyId });
            }

            return View(scene);
        }

        // ---------------- Delete ----------------
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var story = await _storyRepository.GetStoryById(id);
            if (story == null)
                return NotFound();

            return View(story);
        }

        // Use ActionName("Delete") so forms that post to asp-action="Delete" work.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _storyRepository.Delete(id);
            return RedirectToAction(nameof(TableStory));
        }
    }
}
