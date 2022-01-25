using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Win21Pages.Data;
using Win21Pages.Models;
using Microsoft.Extensions.Configuration;

namespace Win21Pages.Pages
{
    public class UserModel : PageModel
    {
        private readonly ILogger<UserModel> _logger;
        private readonly IDbContextFactory<PostContext> _dbContextFactory;
        private readonly ApplicationDbContext _userContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration Configuration;

        public UserModel(ILogger<UserModel> logger,
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
        public PaginatedList<IdentityUser> UserList { get; set; }
        public string userId { get; set; }
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


            await using (var ctx = _userContext)
            {
                IQueryable<IdentityUser> usersIQ = from s in ctx.Users
                                    select s;
                usersIQ = usersIQ.OrderByDescending(n => n.UserName);
                var pageSize = Configuration.GetValue("PostsPageSize", 4);
                UserList = await PaginatedList<IdentityUser>.CreateAsync(
                    usersIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            }
            await using (var ctx = _dbContextFactory.CreateDbContext())
            {
                foreach (var _user in UserList)
                {


                    IQueryable<Post> postsIQ = from s in ctx.Posts
                        select s;

                    postsIQ = postsIQ.Where(u => u.UserId == _user.Id);
                    postsIQ = postsIQ.OrderByDescending(d => d.UploadDate);

                    _user.NormalizedUserName = postsIQ.Count().ToString();

                }
            }

        }
    }
}
