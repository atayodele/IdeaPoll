﻿using IdeaSolution.Data;
using IdeaSolution.Data.IGeneric.Auth;
using IdeaSolution.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Services.Generic
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AuthRepository(DataContext context, 
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public Task<AppUser> Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> Register(AppUser user, string password)
        {
            throw new NotImplementedException();
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