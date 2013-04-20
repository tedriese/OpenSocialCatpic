using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catpic.Gadgets.Containers;
using NUnit.Framework;
using Catpic.Utils;
using Catpic.Utils.Configuration;

namespace Catpic.Gadgets.Tests
{
    [TestFixture]
    public class FeatureBuilderTests
    {
        [Test(Description = "Tests whether shindig's feature set can be created")]
        public void CanBuildFeatureSet()
        {
            //NOTE: assume that first feature in config is shindig's
            var configNode = ConfigSettings.Instance.GetSection("features/set");
            IFeatureSet featureSet = ObjectCreator.Create<FeatureSet>(configNode);
            Assert.AreNotEqual(0, featureSet.Features.Count());
            Assert.IsTrue(featureSet.Features.Any(f => f.Name == "shindig-container"));
        }
    }
}
