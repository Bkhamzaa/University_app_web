using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University_web_app.Models;

namespace University_web_app.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // Register POST
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new Models.Users { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewData["Layout"] = null; 
            return View(model);
        }

        // Login GET
        public IActionResult Login()
        {
            ViewData["IsLoginPage"] = true; // Set flag for login page
            return View();
        }

        public IActionResult Register()
        {
            ViewData["IsRegisterPage"] = true; // Set flag for register page
            return View();
        }
        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("index", "Dashboard");
                    }

                    ModelState.AddModelError("", "Invalid login attempt.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            ViewData["IsLoginPage"] = true; // Set flag to disable layout
            return View(model); // Return view with validation errors
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
