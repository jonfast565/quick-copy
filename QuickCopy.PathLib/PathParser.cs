using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace QuickCopy.PathLib
{
    public class PathParser
    {
        public PathParser(string path)
        {
            Segment = BuildSegments(path);
        }

        public PathSegment Segment { get; set; }

        private static string NormalizePath(string path)
        {
            var normalizedPath = path
                .Replace(Splitters.WindowsSplitter, Splitters.Splitter)
                .Replace(Splitters.UnixSplitter, Splitters.Splitter);
            return normalizedPath;
        }

        private static PathSegment BuildSegments(string path)
        {
            var normalized = NormalizePath(path);
            var split = normalized
                .Split(Splitters.Splitter, StringSplitOptions.RemoveEmptyEntries)
                .Reverse();

            PathSegment nextSegment = null;
            foreach (var seg in split)
            {
                var newItem = new PathSegment
                {
                    Next = nextSegment,
                    Name = seg
                };

                nextSegment = newItem;
            }

            return nextSegment;
        }

        public PathSegment GetDifferingSegment(PathParser p)
        {
            var otherSegmentList = p.Segment.GetRemainingSegments();
            var mySegments = Segment.GetRemainingSegments();

            var segments = mySegments
                .ZipLongest(otherSegmentList,
                    (my, other) => (my, other));

            var result =
                (from segment in segments
                    where !(segment.my?.Name ?? string.Empty)
                        .Equals(segment.other?.Name ?? string.Empty,
                            StringComparison.InvariantCultureIgnoreCase)
                    select segment.other).FirstOrDefault();

            return result;
        }

        public PathParser AppendSegment(string newSegment)
        {
            var segmentParser = new PathParser(newSegment);
            var last = GetLast();
            last.Next = segmentParser.Segment;
            return this;
        }

        private PathSegment GetLast()
        {
            var initialSegment = Segment;
            var segment = initialSegment;

            var queue = new Stack<PathSegment>();
            while (segment != null)
            {
                queue.Push(segment);
                segment = segment.Next;
            }

            var last = queue.Pop();
            return last;
        }

        public PathSegment RemoveLast()
        {
            var initialSegment = Segment;
            var segment = initialSegment;

            var queue = new Stack<PathSegment>();
            while (segment != null)
            {
                queue.Push(segment);
                segment = segment.Next;
            }

            queue.Pop();
            var secondToLast = queue.Pop();
            secondToLast.Next = null;

            return initialSegment;
        }
    }
}