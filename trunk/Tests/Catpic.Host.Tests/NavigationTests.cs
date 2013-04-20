using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Catpic.Host.Engine.Navigation;
using NUnit.Framework;

namespace Catpic.Host.Tests
{
    [TestFixture]
    public class NavigationTests
    {
        private const string PortalNavigationDataSource = "PortalNavigation.xml";
        private const string SocialNavigationDataSource = "SocialNavigation.xml";

        #region Portal
        [Test]
        public void PortalCanBuildFromXml()
        {
            var navProvider = GetProvider(PortalNavigationDataSource);
            var root = navProvider.GetRoot();

            Assert.IsNotNull(root);
            Assert.AreEqual(7, root.Children.Count());
        }

        [Test]
        public void PortalCanGetTopByInventory()
        {
            INavigationService service = new NavigationService(GetProvider(PortalNavigationDataSource));

            INavigationNode node = service.GetNavigation("Portal/Home");

            Assert.IsNotNull(node);
            Assert.AreEqual(7, node.Children.Count());
        }

        [Test]
        public void PortalCanGetSubByInventory()
        {
            INavigationService service = new NavigationService(GetProvider(PortalNavigationDataSource));

            INavigationNode node = service.GetNavigation("Portal/Home/Index");
            //TODO improve test: check isCurrent property
            Assert.IsNotNull(node);
        }
        #endregion

        #region Social

        [Test]
        public void SocialCanBuildFromXml()
        {
            var navProvider = GetProvider(SocialNavigationDataSource);
            var root = navProvider.GetRoot();

            Assert.IsNotNull(root);
            Assert.AreEqual(7, root.Children.Count());
        }

        [Test]
        public void SocialCanGetTopByInventory()
        {
            INavigationService service = new NavigationService(GetProvider(SocialNavigationDataSource));

            INavigationNode node = service.GetNavigation("Social/Profile");

            Assert.IsNotNull(node);
            Assert.AreEqual(7, node.Children.Count());
        }

        [Test]
        public void SocialCanGetSubByInventory()
        {
            INavigationService service = new NavigationService(GetProvider(SocialNavigationDataSource));

            INavigationNode node = service.GetNavigation("Social/Profile/Index");
            //TODO improve test: check isCurrent property
            Assert.IsNotNull(node);
        }

        #endregion


        private static INavigationProvider GetProvider(string navDataSource)
        {
            using (var file = File.Open(navDataSource, FileMode.Open))
            {
                return new XmlNavigationProvider(file);
            }
        }
    }
}
