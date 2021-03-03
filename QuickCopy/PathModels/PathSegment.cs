using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickCopy.PathModels
{
    public class PathSegment
    {
        public string Name { get; set; }
        public PathSegment Next { get; set; }

        public List<PathSegment> GetRemainingSegments()
        {
            var results = new List<PathSegment> {this};
            var currentSegment = this;
            while (currentSegment.Next != null)
            {
                results.Add(currentSegment.Next);
                currentSegment = currentSegment.Next;
            }

            return results;
        }

        public string GetSegmentString(char separator = '/')
        {
            var remainingSegments = GetRemainingSegments();
            var result = remainingSegments.Aggregate($"{separator}",
                    (current, segment) => current + segment.Name + $"{separator}")
                .TrimEnd(separator)
                .TrimStart(separator);
            return result;
        }

        public int GetSegmentLength()
        {
            return GetSegmentString('/').Split('/').Length;
        }

        public bool Contains(PathSegment folderSegment)
        {
            var str1 = GetSegmentString();
            var str2 = folderSegment.GetSegmentString();
            return str1.Contains(str2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}