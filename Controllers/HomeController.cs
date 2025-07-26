using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Messenger.Models;

namespace Messenger.Controllers;

[RequireHttps]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserService _userService;
    private readonly MessageService _messageService;

    public HomeController(ILogger<HomeController> logger, UserService userService, MessageService messageService)
    {
        _logger = logger;
        _userService = userService;
        _messageService = messageService;

        _userService.LoadAllUsers();
        _messageService.LoadAllMessages();
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("userid") != null)
            return Redirect("/messenger");
        return View();
    }

    [Route("about")]
    public IActionResult About()
    {
        return View();
    }

    [Route("messenger")]
    public IActionResult Messenger()
    {
        if (HttpContext.Session.GetInt32("userid") == null)
            return Redirect("/");
        return View();
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromForm] string username, [FromForm] string pwd)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pwd))
        {
            return Redirect("/?p=login&error=1");
        }

        int? userId = UserModel.GetUserId(username, pwd);

        if (userId == null) return Redirect("/?p=login&error=2");

        HttpContext.Session.SetInt32("userid", userId.Value);
        return Redirect($"/messenger");
    }

    [HttpPost]
    [Route("signup")]
    public IActionResult Signup([FromForm] string username, [FromForm] string pwd, [FromForm] string confirmpwd)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pwd) || string.IsNullOrWhiteSpace(confirmpwd))
            return Redirect("/?p=signup&error=1");

        username = username.Trim();
        pwd = pwd.Trim();
        confirmpwd = confirmpwd.Trim();

        if (pwd != confirmpwd)
            return Redirect("/?p=signup&error=2");

        if (!UserModel.ValidPassword(pwd))
            return Redirect("/?p=signup&error=3");

        if (!UserModel.ValidUsername(username))
            return Redirect("/?p=signup&error=4");

        if (UserModel.UserWithNameExists(username))
            return Redirect("/?p=signup&error=5");

        UserModel user = new UserModel(username, pwd);

        _userService.RegisterUser(user);

        HttpContext.Session.SetInt32("userid", user.GetId());
        return Redirect("/messenger");
    }

    [Route("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("userid");
        return Redirect("/");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
