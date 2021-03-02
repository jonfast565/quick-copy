﻿using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;

namespace QuickCopy.PathModels
{
    public class PathParser
    {
        public PathParser(string path)
        {
            SegmentList = BuildSegments(path);
        }

        public PathSegment SegmentList { get; set; }

        private static string NormalizePath(string path)
        {
            var normalizedPath = path.Replace('\\', '/');
            return normalizedPath;
        }

        private static PathSegment BuildSegments(string path)
        {
            var normalized = NormalizePath(path);
            var split = normalized
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
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
            var otherSegmentList = p.SegmentList.GetRemainingSegments();
            var mySegments = SegmentList.GetRemainingSegments();

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

        public PathSegment RemoveLast()
        {
            var initialSegment = SegmentList;
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