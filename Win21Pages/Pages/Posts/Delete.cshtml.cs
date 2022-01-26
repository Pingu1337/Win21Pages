using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Win21Pages.Data;
using Win21Pages.Models;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Win21Pages.Pages.Posts
{
    public class DeleteModel : PageModel
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly IDbContextFactory<PostContext> _dbContextFactory;
        private readonly ApplicationDbContext _userContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        public DeleteModel(ILogger<DeleteModel> logger,
            IDbContextFactory<PostContext> dbContextFactory,
            ApplicationDbContext userContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _userContext = userContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }
        [BindProperty(SupportsGet = true)]
        public IdentityUser SignedInUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public Post? RemovablePost { get; set; }
        public string? PostUserId { get; set; }
        public string? Message { get; set; }
        public string? Test { get; set; }
        public async Task OnGet(int postId, bool HasBeenDeleted)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                SignedInUser = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

                

                if (SignedInUser != null)
                {
                    await using (var ctx = _dbContextFactory.CreateDbContext())
                    {
                        RemovablePost = await ctx
                            .Posts
                            .Where(p => p.Id == postId)
                            .Where(u => u.UserId == SignedInUser.Id)
                            .FirstOrDefaultAsync();

                        if (!HasBeenDeleted)
                        {
                        PostUserId = ctx.Posts.Find(postId).UserId;
                        }
                    }
                }
                if (RemovablePost == null && SignedInUser.Id != PostUserId)
                {
                    Message = "ACCESS DENIED";
                }
                if (HasBeenDeleted)
                {
                    Message = "Post Removed!";
                }                
                if (HasBeenDeleted && RemovablePost != null)
                {
                    Message = "An unexpected error occurred... try again later...";
                }
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            RemoveDirectory removeDirectory = new RemoveDirectory();
            bool deleted = false;
            if (id != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

                await using (var ctx = _dbContextFactory.CreateDbContext())
                {
                    var DeletePost =  await ctx.Posts.FindAsync(id);

                    if (DeletePost != null && DeletePost.UserId == currentUser.Id)
                    {
                        var deletePath = Path.Combine(
                            _env.ContentRootPath,"UserData", currentUser.Id, DeletePost.PostName);
                        var IsRemoved = await removeDirectory.DeleteDirectory(deletePath);
                        if (IsRemoved)
                        {
                            ctx.Posts.Remove(DeletePost);
                            await ctx.SaveChangesAsync();
                            deleted = IsRemoved;
                        }
                    }
                }
            }
            return RedirectToRoute("", new {postId = id, HasBeenDeleted = deleted });
        }
    }
}
