using IdeaSolution.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Data.IGeneric
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhoto(long id);
        Task<Photo> GetMainPhotoForUser(string userId);
    }
}
