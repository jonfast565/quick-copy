using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using QuickCopy.PathModels;

namespace QuickCopy.Tests
{
    public class Tests
    {
        [Test]
        public void TestRemainingPaths()
        {
            var pathParser1 = new PathParser("C:\\Users\\jfast\\Desktop\\");
            var pathParser2 = new PathParser("C:\\Users\\jfast\\Desktop\\Repos\\something.txt");
            var segment = pathParser1.GetDifferingSegment(pathParser2);
            Assert.AreEqual(segment.GetRemainingSegments().Count, 2);
        }

        [Test]
        public void TestPathContains()
        {
            var pathParser1 = new PathParser("C:\\Users\\jfast\\Desktop\\Repos\\");
            var pathParser2 = new PathParser("Desktop\\Repos");
            Assert.IsTrue(pathParser1.SegmentList.Contains(pathParser2.SegmentList));
        }
    }
}