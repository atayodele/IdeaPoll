using IdeaSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Data.IGeneric.Auth
{
    public interface IAuthRepository
    {
        Task<AppUser> Login(string email, string password);
        Task<bool> UserExists(string email);
        Task<IEnumerable<IdentityRole>> GetRoles();
        Task<IdentityRole> GetRole(string id);
    }
}
