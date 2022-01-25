using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Win21Pages.Data;
using Win21Pages.Models;

namespace Win21Pages.Pages
{
    public class User : PageModel
    {
        private readonly ILogger<User> _logger;
        private readonly IDbContextFactory<PostContext> _dbContextFactory;
        private readonly ApplicationDbContext _userContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration Configuration;

        public User(ILogger<User> logger,
            IDbContextFactory<PostContext> dbContextFactory,
            ApplicationDbContext userContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _userContext = userContext;
            _userManager = userManager;
            _signInManager = signInManager;
            Configuration = configuration;
        }

        [BindProperty]
        public PaginatedList<Post>? postList { get; set; }
        public List<Post>? totalPostList { get; set; }
        [BindProperty]
        public IdentityUser PostUser { get; set; }


        public async Task OnGet(string userName, int? pageIndex)
        {

            var user = await _userManager.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
            string userId;
            if (user != null)
            {
                PostUser = user;
                userId = user.Id;
            }
            else
            {
                userId = null;
            }

            await using (var ctx = _dbContextFactory.CreateDbContext())
            {
                totalPostList = await ctx
                    .Posts
                    .OrderByDescending(d => d.UploadDate)
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
            }

            if (totalPostList != null)
            {

                await using (var ctx = _dbContextFactory.CreateDbContext())
                {
                    IQueryable<Post> postsIQ = from s in ctx.Posts
                        select s;
                    postsIQ = postsIQ.Where(u => u.UserId == userId);

                    postsIQ = postsIQ.OrderByDescending(d => d.UploadDate);

                    postList = await PaginatedList<Post>.CreateAsync(
                        postsIQ.AsNoTracking(), pageIndex ?? 1, 10);
                }
            }
        }
    }
}