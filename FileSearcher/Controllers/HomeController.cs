using FileSearcher.Models;
using Microsoft.AspNetCore.Mvc;
using FileSearcher.Data;
using System.Diagnostics;

namespace FileSearcher.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoContext _mongoContext;

        public HomeController(ILogger<HomeController> logger, MongoContext mongoContext)
        {
            _logger = logger;
            _mongoContext = mongoContext;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            List<MyFile> files;
            if (!string.IsNullOrEmpty(searchString))
            {
                files = _mongoContext.Search(searchString);
            }
            else
            {
                files = await _mongoContext.GetFiles();
            }
            var fileVM = new FileViewModel
            {
                Files = files.OrderByDescending(x => x.LastAccessDateTime).ToList()
            };

            return View(fileVM);
        }
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myFile = _mongoContext.GetFile(id);
            if (myFile == null)
            {
                return NotFound();
            }

            return View(myFile);
        }
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myFile = _mongoContext.GetFile(id);
            if (myFile == null)
            {
                return NotFound();
            }
            return View(myFile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Tags")] MyFile file)
        {
            var myFile = _mongoContext.GetFile(id);
            if (myFile == null)
            {
                return NotFound();
            }
            myFile.Title = file.Title;
            myFile.Tags = file.Tags;

            if (ModelState.IsValid)
            {
                _mongoContext.Update(myFile);
                return RedirectToAction(nameof(Index));
            }
            return View(myFile);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}