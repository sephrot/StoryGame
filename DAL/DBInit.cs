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

            // --- SAVE ---
            context.Stories.AddRange(story1, story2);
            context.SaveChanges();
        }
    }
}
