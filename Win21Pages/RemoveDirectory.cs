using System.Configuration;
using Microsoft.Extensions.Hosting.Internal;

namespace Win21Pages
{
    public class RemoveDirectory
    {
        public async Task<bool> DeleteDirectory(string postPath)
        {


            string path = postPath;

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            if (!Directory.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
