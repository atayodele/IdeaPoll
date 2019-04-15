using IdeaSolution.Data;
using IdeaSolution.Data.IGeneric.IdeaRepo;
using IdeaSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Services.Generic
{
    public class IdeaRepository : IIdeaRepository
    {
        private readonly DataContext _context;

        public IdeaRepository(DataContext context)
        {
            _context = context;
        }

        public int Count(int id)
        {
            return _context.Ideas.Count();
        }

        public async Task<IEnumerable<Idea>> GetAll()
        {
            var idea = await _context.Ideas.ToListAsync();
            return idea;
        }

        public async Task<Idea> GetIdea(long id) 
        {
            var idea = await _context.Ideas.FirstOrDefaultAsync(x => x.Id == id);
            return idea;
        }

        public async Task<IEnumerable<Idea>> GetUserIdeas(string userId)
        {
            return await _context.Ideas.OrderByDescending(u => u.Id)
                .Where(c => c.UserId == userId).ToListAsync();
        }
    }
}
