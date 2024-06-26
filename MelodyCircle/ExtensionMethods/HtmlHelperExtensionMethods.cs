﻿using MelodyCircle.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MelodyCircle.ExtensionMethods
{
    public static class HtmlHelperExtensionMethods
    {
        public static string Translate(this IHtmlHelper helper, string key)
        {
            IServiceProvider services = helper.ViewContext.HttpContext.RequestServices;
            SharedViewLocalizer localizer = services.GetRequiredService<SharedViewLocalizer>();
            string result = localizer[key];
            return result;
        }
    }
}