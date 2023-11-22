using KernelHelpBot.Models.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using KernelHelpBot.Models.People_Information;
using System.Security.Claims;

namespace KernelHelpBot.Controllers
{
    public class AccountController:Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
          
            if (ModelState.IsValid)
            {
               
                if (model.Email=="Admin"&& model.Password=="ZernaNet2018")
                {
                    await Authenticate(model.Email); // аутентификация
                    if(model.RefererUrl != null && model.RefererUrl != "")
                        return Redirect(model.RefererUrl);
                    return RedirectToAction("Index", "Home");

                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
         
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
