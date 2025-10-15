using System;
using System.Linq; // for LINQ Select/First
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoryGame.DAL;
using StoryGame.Models;
using System.Collections.Generic;

namespace StoryGame.Controllers
{
    // Absolute base route: everything under /Story
    [Route("/Story")]
    public class StoryController : Controller
    {
        private readonly IStoryRepository _storyRepository;

        public StoryController(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }

        // ---------- TABLE VIEW ----------
        [HttpGet("TableStory")]
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

        // ---------- CREATE STORY ----------
        [HttpGet("Create")]
        public IActionResult Create() => View();

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Story story)
        {
            if (!ModelState.IsValid) return NotFound();

            await _storyRepository.Create(story);
            Console.WriteLine($"Added: Id:{story.StoryId} {story.Text} to database");
            return RedirectToAction("TableStory", "Story");
        }

        // ---------- DETAILS ----------
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var story = await _storyRepository.GetStoryById(id);
            if (story == null) return NotFound();
            return View(story);
        }

        // ---------- STORY LIST ----------
        [HttpGet("StoryView")]
        public async Task<IActionResult> StoryView()
        {
            var stories = await _storyRepository.GetAllStories();
            return View(stories);
        }

        // ---------- PLAY STORY ----------
        [HttpGet("Story/{id:int}")]
        public async Task<IActionResult> Story(int id)
        {
            var story = await _storyRepository.GetStoryById(id);
            if (story == null) return NotFound();

            if (story.ScenesList == null || story.ScenesList.Count == 0)
                story.ScenesList = new List<Scene>();

            var currentScene = story.ScenesList.First();
            return View(currentScene);
        }

        [HttpPost("Story")]
        public async Task<IActionResult> Story(int storyId, int nextScene, int thisScene)
        {
            var story = await _storyRepository.GetStoryById(storyId);
            if (story == null) return NotFound();

            var scene = story.ScenesList.FirstOrDefault(s => s.SceneId == nextScene);
            return View(scene);
        }

        // ---------- PING (debug) ----------
        [HttpGet("ping")]
        public IActionResult Ping() => Content("pong"); // open /Story/ping to verify routing

        // ---------- SCENE TREE DATA (AJAX) ----------
        // Works with BOTH: /Story/TreeData/1  and  /Story/TreeData?id=1
        [HttpGet("TreeData/{id:int}")]
        [HttpGet("TreeData")]
        [Produces("application/json")]
        public async Task<IActionResult> TreeData([FromQuery] int? id, [FromRoute(Name = "id")] int? routeId = null)
        {
            int storyId = id ?? routeId ?? 0;
            if (storyId <= 0)
                return NotFound(new { error = "Invalid story id" });

            // Get story with scenes and choices
            var story = await _storyRepository.GetStoryById(storyId);
            if (story == null)
                return NotFound(new { error = "Story not found" });

            var scenes = story.ScenesList ?? new List<Scene>();
            var byId = scenes.ToDictionary(s => s.SceneId);

            // 1️⃣ Build incoming edge map (which scenes lead to which)
            var incoming = new Dictionary<int, int>();
            foreach (var sc in scenes)
            {
                var choices = sc.ChoiceList ?? new List<Choice>();
                foreach (var ch in choices)
                {
                    if (ch.NextSceneId.HasValue && byId.ContainsKey(ch.NextSceneId.Value))
                    {
                        var child = ch.NextSceneId.Value;
                        incoming[child] = incoming.TryGetValue(child, out var c) ? c + 1 : 1;
                    }
                }
            }

            // 2️⃣ Identify root (no incoming edges)
            var root = scenes.FirstOrDefault(s => !incoming.ContainsKey(s.SceneId)) ?? scenes.FirstOrDefault();
            if (root == null)
                return Json(new { nodes = Array.Empty<object>() });

            // 3️⃣ BFS traversal to assign parent relationships
            var parent = new Dictionary<int, int?> { [root.SceneId] = null };
            var queue = new Queue<int>();
            queue.Enqueue(root.SceneId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var current = byId[currentId];
                var choices = current.ChoiceList ?? new List<Choice>();

                foreach (var ch in choices)
                {
                    if (!ch.NextSceneId.HasValue) continue;
                    var childId = ch.NextSceneId.Value;
                    if (!byId.ContainsKey(childId)) continue;

                    if (!parent.ContainsKey(childId))
                    {
                        parent[childId] = currentId;
                        queue.Enqueue(childId);
                    }
                }
            }

            // 4️⃣ Build output data for JS
            var nodes = scenes.Select(s => new
            {
                id = s.SceneId,
                title = s.Text,
                parentId = parent.TryGetValue(s.SceneId, out var p) ? p : (int?)null
            }).ToList();

            Console.WriteLine($"TreeData: {nodes.Count} scenes built for story {storyId}");
            return Json(new { nodes });
        }
    }
}
