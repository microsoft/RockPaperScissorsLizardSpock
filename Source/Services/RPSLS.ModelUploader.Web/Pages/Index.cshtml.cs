using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace RPSLS.ModelUploader.Web
{
    public class UploadModel : PageModel
    {
        private readonly Settings _settings;
        private IWebHostEnvironment _environment;

        public UploadModel(IWebHostEnvironment environment, IOptions<Settings> settings)
        {
            _environment = environment;
            _settings = settings.Value;
        }

        [BindProperty]
        public IFormFile Model { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var zip = new ZipArchive(Model.OpenReadStream(), ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (string.IsNullOrWhiteSpace(entry.Name))
                    {
                        continue;
                    }

                    using (var entryStream = entry.Open())
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, _settings.UploadPath, entry.Name);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await entryStream.CopyToAsync(fileStream);
                        }
                    }
                }
            }

            return RedirectToPage("Uploaded");
        }
    }
}