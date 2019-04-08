using IdeaSolution.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Data.IGeneric.IdeaRepo
{
    public interface IIdeaRepository
    {
        Task<IEnumerable<Idea>> GetAll();
        Task<Idea> GetIdea(long id); 
        Task<IEnumerable<Idea>> GetUserIdeas(string userId); 
    }
}
