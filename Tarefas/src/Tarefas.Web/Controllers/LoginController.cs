using System;
using Tarefas.DTO;
using Tarefas.DAO;
using AutoMapper;
using Tarefas.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Tarefas.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioDAO _usuarioDAO;
        private readonly IMapper _mapper;

        public LoginController(IUsuarioDAO usuarioDAO, IMapper mapper)
        {
            _usuarioDAO = usuarioDAO;
            _mapper = mapper;   
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Index(UsuarioViewModel usuarioViewModel)
        {
            UsuarioDTO user;

            if(ModelState.IsValid)
            {
                try
                {
                    user = _usuarioDAO.Autenticar(usuarioViewModel.Email, usuarioViewModel.Senha);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Nome),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    };
                    
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        IsPersistent = true,
                        RedirectUri = "/Login"
                    };
                    
                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                 }

                catch (System.Exception)
                {
                              
                ModelState.AddModelError("email", "Verifique se informou seus dados corretamente");
                return View();
                } 
            }

            return LocalRedirect("/Home");
        }

        public IActionResult Sair()
        {

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/Login");

        }

    }
}
