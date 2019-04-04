using IdeaSolution.Data;
using IdeaSolution.Data.IGeneric.Auth;
using IdeaSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Services.Generic
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthRepository(DataContext context, 
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityRole> GetRole(string id)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);
            return role;
        }

        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<AppUser> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null && await _userManager.IsLockedOutAsync(user))
                return null;
            if (!await _userManager.CheckPasswordAsync(user, password))
                return null;
            //Auth successfull
            return user;
        }

        public async Task<bool> UserExists(string email)
        {
            var user = await _context.Users.AnyAsync(x => x.Email == email);
            if (user)
                return true;
            return false;
        }
    }
}
