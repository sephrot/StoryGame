using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StoryGame.Models;

namespace StoryGame.DAL
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<StoryDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Stories.Any())
                return;

            // --- STORY 1 ---
            var story1 = new Story
            {
                Text = "The Adventure Begins",
                ScenesList = new List<Scene>(),
            };

            var forestScene = new Scene
            {
                Text = "You wake up in a dark forest.",
                IsFirstScene = true,
                IsFinalScene = false,
                Story = story1,
            };

            var swordScene = new Scene
            {
                Text = "You find a mysterious sword.",
                IsFinalScene = false,
                Story = story1,
            };

            var dragonScene = new Scene
            {
                Text = "You defeat the dragon and win.",
                IsFinalScene = true,
                Story = story1,
            };

            forestScene.ChoiceList.Add(
                new Choice
                {
                    Text = "Look around for help",
                    NextScene = swordScene,
                    ThisScene = forestScene,
                }
            );
            forestScene.ChoiceList.Add(
                new Choice
                {
                    Text = "Run deeper into the woods",
                    NextScene = dragonScene,
                    ThisScene = forestScene,
                }
            );

            swordScene.ChoiceList.Add(
                new Choice
                {
                    Text = "Take the sword and face the dragon",
                    NextScene = dragonScene,
                    ThisScene = swordScene,
                }
            );

            story1.ScenesList.AddRange(new[] { forestScene, swordScene, dragonScene });

            // --- STORY 2 ---
            var story2 = new Story { Text = "A Day in the City", ScenesList = new List<Scene>() };

            var streetScene = new Scene
            {
                Text = "You walk down the busy street.",
                IsFinalScene = false,
                IsFirstScene = true,
                Story = story2,
            };

            var friendScene = new Scene
            {
                Text = "You meet an old friend.",
                IsFinalScene = false,
                Story = story2,
            };

            var cafeScene = new Scene
            {
                Text = "You enjoy a coffee at the cafe.",
                IsFinalScene = true,
                Story = story2,
            };

            streetScene.ChoiceList.Add(
                new Choice
                {
                    Text = "Say hi to your friend",
                    NextScene = friendScene,
                    ThisScene = streetScene,
                }
            );
            streetScene.ChoiceList.Add(
                new Choice
                {
                    Text = "Ignore them and go to the cafe",
                    NextScene = cafeScene,
                    ThisScene = streetScene,
                }
            );

            friendScene.ChoiceList.Add(
                new Choice
                {
                    Text = "Catch up over coffee",
                    NextScene = cafeScene,
                    ThisScene = friendScene,
                }
            );

            story2.ScenesList.AddRange(new[] { streetScene, friendScene, cafeScene });

            // --- STORY 3 ---
            var story3 = new Story
            {
                Text = "The Lost Colony on Mars",
                ScenesList = new List<Scene>(),
            };

            // Scene 1
            var landingScene = new Scene
            {
                Text = "Your ship lands on the red dunes of Mars. The colony has gone silent.",
                IsFirstScene = true,
                Story = story3,
            };

            // Scene 2–10
            var baseScene = new Scene
            {
                Text = "You reach the colony gates. They are sealed.",
                Story = story3,
            };
            var powerScene = new Scene
            {
                Text = "You find the power core offline.",
                Story = story3,
            };
            var radioScene = new Scene
            {
                Text = "You try to contact Earth, but static answers.",
                Story = story3,
            };
            var roverScene = new Scene
            {
                Text = "You take the rover to explore nearby ruins.",
                Story = story3,
            };
            var cavernScene = new Scene
            {
                Text = "You enter a cavern filled with strange markings.",
                Story = story3,
            };
            var aiScene = new Scene
            {
                Text = "An AI awakens and demands your identification.",
                Story = story3,
            };
            var alienScene = new Scene
            {
                Text = "You discover an alien artifact pulsing with light.",
                Story = story3,
            };
            var betrayalScene = new Scene
            {
                Text = "Your crewmate betrays you, taking the artifact.",
                Story = story3,
            };
            var finalGoodScene = new Scene
            {
                Text =
                    "You restore power, revive the colony, and contact Earth. Humanity is saved.",
                IsFinalScene = true,
                Story = story3,
            };
            var finalBadScene = new Scene
            {
                Text = "You activate the artifact. The colony vanishes. Silence forever.",
                IsFinalScene = true,
                Story = story3,
            };

            // Choices
            landingScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Head to the colony gates",
                        NextScene = baseScene,
                        ThisScene = landingScene,
                    },
                    new Choice
                    {
                        Text = "Inspect the ship systems",
                        NextScene = powerScene,
                        ThisScene = landingScene,
                    },
                    new Choice
                    {
                        Text = "Try to send a signal home",
                        NextScene = radioScene,
                        ThisScene = landingScene,
                    },
                }
            );

            baseScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Force the gates open",
                        NextScene = roverScene,
                        ThisScene = baseScene,
                    },
                    new Choice
                    {
                        Text = "Search for another entrance",
                        NextScene = cavernScene,
                        ThisScene = baseScene,
                    },
                    new Choice
                    {
                        Text = "Return to ship for tools",
                        NextScene = powerScene,
                        ThisScene = baseScene,
                    },
                }
            );

            powerScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Repair the core",
                        NextScene = aiScene,
                        ThisScene = powerScene,
                    },
                    new Choice
                    {
                        Text = "Divert power to communications",
                        NextScene = radioScene,
                        ThisScene = powerScene,
                    },
                    new Choice
                    {
                        Text = "Abandon the base",
                        NextScene = roverScene,
                        ThisScene = powerScene,
                    },
                }
            );

            radioScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Boost signal manually",
                        NextScene = aiScene,
                        ThisScene = radioScene,
                    },
                    new Choice
                    {
                        Text = "Move to higher ground",
                        NextScene = roverScene,
                        ThisScene = radioScene,
                    },
                    new Choice
                    {
                        Text = "Return to ship",
                        NextScene = baseScene,
                        ThisScene = radioScene,
                    },
                }
            );

            roverScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Enter the ruins",
                        NextScene = cavernScene,
                        ThisScene = roverScene,
                    },
                    new Choice
                    {
                        Text = "Scan for life",
                        NextScene = aiScene,
                        ThisScene = roverScene,
                    },
                    new Choice
                    {
                        Text = "Return to base",
                        NextScene = baseScene,
                        ThisScene = roverScene,
                    },
                }
            );

            cavernScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Examine the markings",
                        NextScene = alienScene,
                        ThisScene = cavernScene,
                    },
                    new Choice
                    {
                        Text = "Collect samples",
                        NextScene = aiScene,
                        ThisScene = cavernScene,
                    },
                    new Choice
                    {
                        Text = "Retreat to rover",
                        NextScene = roverScene,
                        ThisScene = cavernScene,
                    },
                }
            );

            aiScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Cooperate with the AI",
                        NextScene = alienScene,
                        ThisScene = aiScene,
                    },
                    new Choice
                    {
                        Text = "Try to override it",
                        NextScene = betrayalScene,
                        ThisScene = aiScene,
                    },
                    new Choice
                    {
                        Text = "Shut it down",
                        NextScene = cavernScene,
                        ThisScene = aiScene,
                    },
                }
            );

            alienScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Touch the artifact",
                        NextScene = finalBadScene,
                        ThisScene = alienScene,
                    },
                    new Choice
                    {
                        Text = "Analyze it scientifically",
                        NextScene = finalGoodScene,
                        ThisScene = alienScene,
                    },
                    new Choice
                    {
                        Text = "Hide it and return to the colony",
                        NextScene = betrayalScene,
                        ThisScene = alienScene,
                    },
                }
            );

            betrayalScene.ChoiceList.AddRange(
                new[]
                {
                    new Choice
                    {
                        Text = "Chase the traitor",
                        NextScene = alienScene,
                        ThisScene = betrayalScene,
                    },
                    new Choice
                    {
                        Text = "Try to reason with them",
                        NextScene = finalGoodScene,
                        ThisScene = betrayalScene,
                    },
                    new Choice
                    {
                        Text = "Let them escape",
                        NextScene = finalBadScene,
                        ThisScene = betrayalScene,
                    },
                }
            );

            // Add all scenes
            story3.ScenesList.AddRange(
                new[]
                {
                    landingScene,
                    baseScene,
                    powerScene,
                    radioScene,
                    roverScene,
                    cavernScene,
                    aiScene,
                    alienScene,
                    betrayalScene,
                    finalGoodScene,
                    finalBadScene,
                }
            );

            // --- SAVE ---
            context.Stories.AddRange(story1, story2, story3);
            context.SaveChanges();
        }
    }
}
