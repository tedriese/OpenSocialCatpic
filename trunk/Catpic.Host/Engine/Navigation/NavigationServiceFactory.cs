using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Catpic.Host.Engine.Navigation
{
    public class NavigationServiceFactory
    {
        private static readonly object __lock = new object();
        private static volatile INavigationService __navPortalService;
        private static volatile INavigationService __navSocialService;
        
        /// <summary>
        /// Returns portal navigation service. 
        /// NOTE: this approach simplifies coding but it would be better to refactor this (static isn't testable)
        /// </summary>
        public static INavigationService Portal
        {
            get
            {
                //TODO refactoring
                if(__navPortalService == null)
                {
                    lock (__lock)
                    {
                        if(__navPortalService == null)
                        {
                            using (var file = File.Open(Consts.PortalNavigationMenuPath, FileMode.Open))
                            {
                                var navigationProvider = new XmlNavigationProvider(file);
                                __navPortalService = new NavigationService(navigationProvider);
                            }
                        }
                    }
                }
                return __navPortalService;
            }
        }

        /// <summary>
        /// Returns social navigation service
        /// </summary>
        public static INavigationService Social
        {
            get
            {
                //TODO refactoring
                if (__navSocialService == null)
                {
                    lock (__lock)
                    {
                        if (__navSocialService == null)
                        {
                            using (var file = File.Open(Consts.SocialNavigationMenuPath, FileMode.Open))
                            {
                                var navigationProvider = new XmlNavigationProvider(file);
                                __navSocialService = new NavigationService(navigationProvider);
                            }
                        }
                    }
                }
                return __navSocialService;
            }
        }
    }
}