using System;
using System.Security.Cryptography;

namespace QuickCopy.PathLib
{
    public class FileInfoParserAction
    {
        public FileInfoParserAction(FileInfoParser source, FileInfoParser destination, ActionType type)
        {
            Source = source;
            Destination = destination;
            Type = type;
        }

        public FileInfoParser Source { get; }
        public FileInfoParser Destination { get; }
        public ActionType Type { get; }

        public int GetSourceLength()
        {
            return Source.Segment.GetSegmentLength();
        }

        public int GetDestinationLength()
        {
            return Destination.Segment.GetSegmentLength();
        }

        public string GetDestinationFromSegment(string targetDirectory)
        {
            var pp = new PathParser(targetDirectory);
            var fif = new FileInfoParser(targetDirectory, targetDirectory);
            pp.AppendSegment(Source.Segment.GetSegmentString());
            var destinationSegment = pp.Segment.GetSegmentString();
            if (fif.IsUncPath)
            {
                return "\\\\" + destinationSegment;
            }
            return destinationSegment;
        }
    }
}