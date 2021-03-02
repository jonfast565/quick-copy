namespace QuickCopy.PathModels
{
    public class FileInfoParserAction
    {
        public FileInfoParserAction(FileInfoParser source, FileInfoParser destination, ActionType type)
        {
            ParserSource = source;
            ParserDestination = destination;
            Type = type;
        }

        public FileInfoParser ParserSource { get; }
        public FileInfoParser ParserDestination { get; }
        public ActionType Type { get; }

        public int GetSourceOrDestinationLength()
        {
            var length = 0;
            if (ParserSource == null)
            {
                length = ParserDestination.PathSegmentHead.GetSegmentLength();
            }

            if (ParserDestination == null)
            {
                length = ParserSource.PathSegmentHead.GetSegmentLength();
            }

            return length;
        }
    }
}