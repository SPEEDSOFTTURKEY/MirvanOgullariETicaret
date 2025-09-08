﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var kullanici = SessionHelper.GetObjectFromJson<Kullanicilar>(HttpContext.Session, "Kullanici");

            if (kullanici == null)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            int lisansKalanGun = SessionHelper.GetObjectFromJson<int>(HttpContext.Session, "LisansKalanGun");
            ViewBag.LisansKalanGun = lisansKalanGun;

            base.OnActionExecuting(context);
        }
    }
}
