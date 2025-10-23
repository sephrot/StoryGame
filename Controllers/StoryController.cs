using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoryGame.DAL;
using StoryGame.Models;
using StoryGame.Models.ViewModels; // <-- for WorkspaceViewModel

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

            foreach (var story in stories)
            {
                Console.WriteLine("StoryName: " + story.Text);
                Console.WriteLine("Total Scenes: " + (story.ScenesList?.Count ?? 0));
            }

            return View(stories);
        }

        // ---------------- Workspace (single page) ----------------

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            var all = await _storyRepository.GetAllStories();
            var selected = id.HasValue ? await _storyRepository.GetStoryById(id.Value) : null;


            var vm = new WorkspaceViewModel { Stories = all, Selected = selected };
            return View(vm);
        }


        // POST: /Story/Save  (Create or Update in the same workspace)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Story model)
        {
            if (!ModelState.IsValid)
            {
                var stories = await _storyRepository.GetAllStories();
                var vm = new WorkspaceViewModel { Stories = stories, Selected = model };
                return View("Create", vm);
            }

            if (model.StoryId == 0)
            {
                await _storyRepository.Create(model);
                Console.WriteLine($"Added: Id:{model.StoryId} {model.Text} to database");
            }
            else
            {

                await _storyRepository.Update(model);
                Console.WriteLine($"Updated: Id:{model.StoryId} {model.Text}");
            }

            return RedirectToAction(nameof(Create), new { id = model.StoryId });
        }


        // ---------------- Details -> redirect to single-page workspace ----------------
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Always use the workspace page
            return RedirectToAction(nameof(Create), new { id });

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
                TempData["Info"] = "This story has no scenes yet.";
                return RedirectToAction(nameof(Create), new { id });
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
                return RedirectToAction(nameof(Create), new { id = storyId });
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _storyRepository.Delete(id);
            // After delete, return to workspace with no selection
            return RedirectToAction(nameof(Create));
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> EditorPartial(int id)
        {
            var story = await _storyRepository.GetStoryById(id);
            if (story == null) return NotFound();

            var html = $@"
<div class='editor-top'>
  <h2 style=""margin:0"">{System.Net.WebUtility.HtmlEncode(story.Text)}</h2>
  <p style=""opacity:.7;margin-top:.25rem"">[DEBUG: partial loaded] Story ID: {story.StoryId}</p>
</div>

<div style=""margin-top:1rem;opacity:.9"">
  {(story.ScenesList != null && story.ScenesList.Any()
              ? string.Join("", story.ScenesList.Select(sc => $@"
        <div class='scene-row' style='margin:.35rem 0'>
          • {System.Net.WebUtility.HtmlEncode(sc.Text)}
          <button class='btn-xs js-edit-scene' data-id='{sc.SceneId}'
                  style='margin-left:.5rem;padding:.2rem .5rem;border-radius:6px;border:1px solid rgba(255,255,255,.2);background:transparent;cursor:pointer'>
            Edit
          </button>
        </div>"))
              : "<em>No scenes yet.</em>")}
</div>

<div id='workspace-modal' hidden></div>";
            return Content(html, "text/html");
        }




    }

}
