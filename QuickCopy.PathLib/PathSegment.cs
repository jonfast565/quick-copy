using System.Collections.Generic;
using System.Linq;

namespace QuickCopy.PathLib
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

        public string GetSegmentString(char separator = Splitters.WindowsSplitter)
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
            return GetSegmentString(Splitters.Splitter)
                .Split(Splitters.Splitter).Length;
        }

        public bool ContainsAllOfSegment(PathSegment folderSegment)
        {
            var str1 = GetSegmentString(Splitters.Splitter);
            var str2 = folderSegment.GetSegmentString(Splitters.Splitter);
            var split1 = str1.Split(Splitters.Splitter);
            var split2 = str2.Split(Splitters.Splitter);
            var splitCtr = 0;

            foreach (var t in split1)
            {
                if (split2[splitCtr] == t)
                {
                    splitCtr++;

                    if (splitCtr == split2.Length)
                    {
                        return true;
                    }
                }
                else
                {
                    splitCtr = 0;
                }
            }

            return false;
        }
    }
}