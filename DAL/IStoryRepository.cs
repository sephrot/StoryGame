using StoryGame.Models;

namespace StoryGame.DAL;

public interface IStoryRepository
{
    Task<IEnumerable<Story>> GetEverything();
    Task<IEnumerable<Story>> GetAllStories();
    Task<Story?> GetStoryById(int id);
    Task Create(Story story);
    Task Update(Story story);
    Task<bool> Delete(int id);
    IEnumerable<Scene> GetScenesByStoryId(int storyId);
}
