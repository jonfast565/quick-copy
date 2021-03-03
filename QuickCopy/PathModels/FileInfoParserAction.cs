namespace QuickCopy.PathModels
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
    }
}