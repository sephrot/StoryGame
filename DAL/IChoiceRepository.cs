using StoryGame.Models;

namespace StoryGame.DAL;

public interface IChoiceRepository
{
    Task<IEnumerable<Scene?>> GetAvailableScenesForChoice(int sceneId);
    Task<Choice?> GetChoiceById(int id);
    Task<bool> Create(Choice choice);
    Task<bool> Update(Choice choice);
    Task<bool> Delete(int id);
    Task<Choice?> GetChoiceAsync(int id);
}
