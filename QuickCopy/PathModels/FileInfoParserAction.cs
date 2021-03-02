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

        public int GetSourceLength()
        {
            return ParserSource.PathSegmentHead.GetSegmentLength();
        }

        public int GetDestinationLength()
        {
            return ParserDestination.PathSegmentHead.GetSegmentLength();
        }
    }
}