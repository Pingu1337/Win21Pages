using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Win21Pages.Data;
using Win21Pages.Models;

namespace Win21Pages.Pages.Posts
{
    public class Index : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IDbContextFactory<PostContext> _dbContextFactory;
        private readonly ApplicationDbContext _userContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public Index(ILogger<PrivacyModel> logger,
            IDbContextFactory<PostContext> dbContextFactory,
            ApplicationDbContext userContext,
            UserManager<IdentityUser> userManager,  
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _userContext = userContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public Post? viewPost { get; set; }
        [BindProperty]
        public IdentityUser PostUser { get; set; }
        public IdentityUser SignedInUser { get; set; }


        public async Task OnGet(int postID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            SignedInUser = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            await using (var ctx = _dbContextFactory.CreateDbContext())
            {
                viewPost = await ctx
                    .Posts
                    .Where(p => p.Id == postID)
                    .FirstOrDefaultAsync();
            }

            if (viewPost != null)
            {
                var user =  await _userManager.Users.Where(u => u.Id == viewPost.UserId).FirstOrDefaultAsync();
                if (user != null)
                {
                    viewPost.UserId = user.UserName;
                    PostUser = user;
                }
            }
        }
    }
}