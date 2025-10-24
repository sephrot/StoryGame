using StoryGame.Models;

namespace StoryGame.DAL;

public interface ISceneRepository
{
    Task<IEnumerable<Scene?>> GetAllScenes();
    Task<Story?> GetAllScenesByStoryId(int storyId);
    Task<Scene?> GetSceneById(int id);
    Task<bool> Create(Scene scene);
    Task<bool> Update(Scene scene);
    Task<bool> Delete(int id);
}
