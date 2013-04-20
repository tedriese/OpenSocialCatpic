using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Catpic.Utils;

namespace Catpic.Host.Engine
{
    internal static class Consts
    {
        public static string CanonicalDbPath = @"~/App_Data/canonicaldb.json";
        public static string PortalNavigationMenuPath = FileHelper.ResolvePath(@"~/App_Data/PortalNavigation.xml");
        public static string SocialNavigationMenuPath = FileHelper.ResolvePath(@"~/App_Data/SocialNavigation.xml");

        public const string AnonymousId = "john.doe";
    }
}