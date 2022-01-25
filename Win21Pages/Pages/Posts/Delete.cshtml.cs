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
        public DeleteModel(ILogger<DeleteModel> logger,
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
        [BindProperty(SupportsGet = true)]
        public IdentityUser SignedInUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public Post? RemovablePost { get; set; }
        
        public string? Message { get; set; }
        public string? Test { get; set; }
        public async Task OnGet(int postId)
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
                    }

                }

                if (RemovablePost == null && SignedInUser == null)
                {
                    Message = "ACCESS DENIED";
                }

                if (RemovablePost == null && SignedInUser != null)
                {
                    //[TODO]: style Post Removed message
                    Message = "Post Removed!";
                }
        }

        public async Task<IActionResult> OnPostAsync(int id)  
        {

            //[TODO]: actually remove post from server somehow
            if (id != null)
            {
                await using (var ctx = _dbContextFactory.CreateDbContext())
                {
                    var DeletePost =  await ctx.Posts.FindAsync(id);
                    if (DeletePost != null)
                    {
                        ctx.Posts.Remove(DeletePost);
                        await ctx.SaveChangesAsync();
                    }

                }
            }
            return RedirectToPage();
        }
    }
}
