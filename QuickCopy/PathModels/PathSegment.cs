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

        public bool Contains(PathSegment skipFolderSegmentList)
        {
            /*
            var segment = this;
            var comparisonSegment = skipFolderSegmentList;
            while (segment != null)
            {
                if (comparisonSegment.Name == segment.Name)
                {
                    comparisonSegment.Name;
                }
                segment = Next;
            }
            */
            return false;
        }
    }
}