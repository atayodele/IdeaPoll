using IdeaSolution.Data;
using IdeaSolution.Data.IGeneric;
using IdeaSolution.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Services.Generic
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;

        public PhotoRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Photo> GetMainPhotoForUser(string userId)
        {
            return await _context.Photos
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(long id)
        {
            return await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
