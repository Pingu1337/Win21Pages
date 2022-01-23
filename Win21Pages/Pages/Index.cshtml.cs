using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Identity;
using Win21Pages.Data;
using Win21Pages.Models;

namespace Win21Pages.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDbContextFactory<PostContext> _dbContextFactory;
        private readonly ApplicationDbContext _userContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(ILogger<IndexModel> logger,
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
        public List<Post> Posts { get; set; }
        [BindProperty]
        public List<IdentityUser> Users { get; set; }

        public async Task OnGet()
        {
            
            var user = await _userManager.GetUserAsync(User);

            await using (var ctx = _dbContextFactory.CreateDbContext())
            {
                    Posts = await ctx
                        .Posts
                        .ToListAsync();
            }

            await using (var ctx = _userContext)
            {
                foreach (var post in Posts)
                {
                    var postUser = await ctx
                        .Users
                        .Where(u => u.Id == post.UserId)
                        .FirstOrDefaultAsync();
                    if (postUser != null)
                    {
                        post.UserId = postUser.UserName;
                    }
                    else
                    {
                        post.UserId = "Unknown User";
                    }
                    
                }
            }



        }
    }
}