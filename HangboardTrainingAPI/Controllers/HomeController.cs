using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MyBoardsAPI.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult PrivacyPolicy()
    {
        return View("Privacy");
    }

    public IActionResult EULA()
    {
        return View("EndUserLisenseAgreement");
    }
}