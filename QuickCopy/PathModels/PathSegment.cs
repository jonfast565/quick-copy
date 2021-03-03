using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using MoreLinq;

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
            var str1 = GetSegmentString('|');
            var str2 = folderSegment.GetSegmentString('|');
            var split1 = str1.Split('|');
            var split2 = str2.Split('|');
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