using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using QuickCopy.PathLib;

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
            var pathParser1 = new PathParser("C:\\Users\\jfast\\OneDrive - American College of Cardiology\\Desktop\\Test1\\Logs\\New Text Document.txt");
            var pathParser2 = new PathParser("Logs");
            var result = pathParser1.Segment.ContainsAllOfSegment(pathParser2.Segment);
            Assert.IsTrue(result);
        }
    }
}