using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Messenger.Models;

namespace Messenger.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("about")]
    public IActionResult About()
    {
        return View();
    }

    [Route("messages")]
    public IActionResult Messages()
    {
        int? name = HttpContext.Session.GetInt32("userid");
        return Content($"Hello, {name}");
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromForm] string username, [FromForm] string pwd)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pwd))
        {
            return Redirect("/?login&error=1");
        }

        username = username.Trim();
        pwd = pwd.Trim();

        int? userId = UserModel.GetUserId(username, pwd);

        if (userId == null) return Redirect("/?login&error=2");

        HttpContext.Session.SetInt32("userid", userId.Value);
        return Redirect($"/messages");
    }

    [HttpPost]
    [Route("signup")]
    public IActionResult Signup([FromForm] string username, [FromForm] string pwd, [FromForm] string confirmpwd)
    {
        return Redirect($"/?username={username}&password={pwd}&confirmpwd={confirmpwd}");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
