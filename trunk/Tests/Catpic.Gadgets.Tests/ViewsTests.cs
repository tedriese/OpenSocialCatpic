// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewsTests.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    /// <summary>
    /// Tests  gadget core module view functionality 
    /// </summary>
    [TestFixture]
    public class ViewsTests
    {
        [Test]
        public void CanBuildViewFromDifferentParts()
        {
            StringWriter writer = new StringWriter();
            var task = TestHelper.GetCreateGadgetTask(Path.GetFullPath(TestConsts.ViewGadget), "inline", writer, "canvas");
            task.Wait();
            var output = writer.ToString();
            Assert.IsTrue(output.StartsWith("<div>Part1Part2<script>"));
        }

        [Test]
        public void CanBuildView()
        {
            StringWriter writer = new StringWriter();
            var task = TestHelper.GetCreateGadgetTask(Path.GetFullPath(TestConsts.ViewGadget), "inline", writer, "canvas.error");
            task.Wait();
            var output = writer.ToString();
            Assert.IsTrue(output.StartsWith("<div>error<script>"));
        }
    }
}
