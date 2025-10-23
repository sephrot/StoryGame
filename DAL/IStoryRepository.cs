using System.Collections.Generic;
using System.Threading.Tasks;
using StoryGame.Models;

namespace StoryGame.DAL
{
    public interface IStoryRepository
    {
        
        Task<IEnumerable<Story>> GetAllStories();

        
        Task<IEnumerable<Story>> GetEverything();

       
        Task<Story?> GetStoryById(int id);

        
        Task Create(Story story);

        
        Task Update(Story story);

        
        Task<bool> Delete(int id);

        
        IEnumerable<Scene> GetScenesByStoryId(int storyId);
    }
}
