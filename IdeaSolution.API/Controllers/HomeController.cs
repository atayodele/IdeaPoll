using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeaSolution.Data;
using IdeaSolution.Data.IGeneric.Auth;
using IdeaSolution.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSolution.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthRepository _auth;

        public HomeController(DataContext context, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<AppUser> userManager,
            IAuthRepository auth)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _auth = auth;
        }
        [HttpGet]
        public IActionResult UserCount()
        {
            var user = _context.Users.Count();
            return Ok(user);
        }
        [HttpGet("ideaCount")]
        public IActionResult IdeaCount()
        {
            var idea = _context.Ideas.Count();
            return Ok(idea);
        }
        [HttpGet("roleCount")]
        public IActionResult roleCount()
        {
            var role = _context.Roles.Count();
            return Ok(role);
        }
        [HttpGet("roleCountByUser")]
        public IActionResult roleCountByUser()
        {
             var role = (from r in _context.UserRoles
                        join u in _context.Users on r.UserId equals u.Id
                        group new { r, u } by new { r.RoleId } into grp
                        select new { name = grp.FirstOrDefault().r.RoleId, count = grp.Count() });

            return Ok(role);
        }
    }
}