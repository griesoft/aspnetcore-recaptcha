using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GSoftware.AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReCaptcha.Sample.Models;

namespace ReCaptcha.Sample.Controllers
{
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

        [ValidateRecaptcha(ValidationFailedAction = ValidationFailedAction.ContinueRequest)]
        public IActionResult Privacy(ValidationResponse recaptchaResponse)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
