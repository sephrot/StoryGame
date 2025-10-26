using StoryGame.Models;

namespace StoryGame.DAL;

public interface IStoryRepository
{
    Task<IEnumerable<Story>?> GetEverything();
    Task<IEnumerable<Story>?> GetAllStories();
    Task<Story?> GetStoryById(int id);
    Task<bool> Create(Story story);
    Task<bool> Update(Story story);
    Task<bool> Delete(int id);
}
