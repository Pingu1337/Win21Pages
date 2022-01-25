using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Win21Pages.Data;
using Win21Pages.Models;

namespace Win21Pages.Pages
{
    public class MyUploadsModel : PageModel
    {
            private readonly ILogger<MyUploadsModel> _logger;
            private readonly IDbContextFactory<PostContext> _dbContextFactory;
            private readonly ApplicationDbContext _userContext;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly SignInManager<IdentityUser> _signInManager;
            private readonly IConfiguration Configuration;

            public MyUploadsModel(ILogger<MyUploadsModel> logger,
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
            public string UserName { get; set; }
            public IdentityUser LoggedInUser { get; set; }

            public PaginatedList<Post> PostsList { get; set; }
            public List<Post> totalPostList { get; set; }

            public async Task OnGet(int? pageIndex)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
                UserName = user.UserName;
                LoggedInUser = user;

                using (var ctx = await _dbContextFactory.CreateDbContextAsync())
                {
                    IQueryable<Post> postsIQ = from s in ctx.Posts
                        select s;
                    postsIQ = postsIQ.Where(u => u.UserId == user.Id);

                    postsIQ = postsIQ.OrderByDescending(d => d.UploadDate);

                    PostsList = await PaginatedList<Post>.CreateAsync(
                        postsIQ.AsNoTracking(), pageIndex ?? 1, 10);
                }

                await using (var ctx = _dbContextFactory.CreateDbContext())
                {
                    totalPostList = await ctx
                        .Posts
                        .OrderByDescending(d => d.UploadDate)
                        .Where(p => p.UserId == user.Id)
                        .ToListAsync();
                }
            }
    }
}

