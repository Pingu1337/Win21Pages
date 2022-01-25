using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Win21Pages.Data;
using Win21Pages.Models;
using Microsoft.Extensions.Configuration;

namespace Win21Pages.Pages
{
    public class CollectionModel : PageModel
    {
        private readonly ILogger<CollectionModel> _logger;
        private readonly IDbContextFactory<PostContext> _dbContextFactory;
        private readonly ApplicationDbContext _userContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration Configuration;

        public CollectionModel(ILogger<CollectionModel> logger,
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
        public List<Post> Posts { get; set; }
        [BindProperty]
        public int postId { get; set; }

        public PaginatedList<Post> PostList { get; set; }

        public async Task OnGet(int? pageIndex)
        {


            var user = await _userManager.GetUserAsync(User);
            await using (var ctx = _dbContextFactory.CreateDbContext())
            {
                Posts = await ctx
                    .Posts
                    .OrderByDescending(d => d.UploadDate)
                    .ToListAsync();
            }


            await using (var ctx = _dbContextFactory.CreateDbContext())
            {
                IQueryable<Post> postsIQ = from s in ctx.Posts
                                    select s;
                postsIQ = postsIQ.OrderByDescending(d => d.UploadDate);


                var pageSize = Configuration.GetValue("PostsPageSize", 4);
                PostList = await PaginatedList<Post>.CreateAsync(
                    postsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            }
            await using (var ctx = _userContext)
            {
                foreach (var post in PostList)
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
