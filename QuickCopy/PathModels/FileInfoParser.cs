using System.IO;

namespace QuickCopy.PathModels
{
    public class FileInfoParser
    {
        public FileInfoParser(string path, string baseDirectory)
        {
            var attr = System.IO.File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                IsFile = false;
                Directory = new DirectoryInfo(path);
                PathSegmentHead = new PathParser(baseDirectory).GetDifferingSegment(new PathParser(Directory.FullName));
            }
            else
            {
                IsFile = true;
                File = new FileInfo(path);
                PathSegmentHead = new PathParser(baseDirectory).GetDifferingSegment(new PathParser(File.FullName));
            }
        }

        public PathSegment PathSegmentHead { get; }
        public FileInfo File { get; }
        public DirectoryInfo Directory { get; }
        public bool IsFile { get; }
    }
}