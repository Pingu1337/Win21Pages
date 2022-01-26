using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Win21Pages.Models;

namespace Win21Pages.Pages.Page
{
    public class PageIndexModel : PageModel
    {
        private readonly IDbContextFactory<PostContext> _dbContextFactory;
        public PageIndexModel(IDbContextFactory<PostContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public Post pagePost { get; set; }

        public string Message { get; set; }

        public async Task OnGet(int? postId)
        {
            if (postId != null)
            {
                await using (var ctx = _dbContextFactory.CreateDbContext())
                {
                   pagePost = await ctx.Posts.FindAsync(postId);
                }
            }
            else
            {
                Console.WriteLine("Redirecting to index...");
                Message = "404: Page not found";
            }

        }
    }
}
