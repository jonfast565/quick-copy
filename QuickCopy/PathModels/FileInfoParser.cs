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
                Segment = new PathParser(baseDirectory)
                    .GetDifferingSegment(
                        new PathParser(Directory.FullName));
                IsUncPath = baseDirectory.IsUnc();
            }
            else
            {
                IsFile = true;
                File = new FileInfo(path);
                Segment = new PathParser(baseDirectory)
                    .GetDifferingSegment(
                        new PathParser(File.FullName));
                IsUncPath = baseDirectory.IsUnc();
            }
        }

        public PathSegment Segment { get; }
        public FileInfo File { get; }
        public DirectoryInfo Directory { get; }
        public bool IsFile { get; }
        public bool IsUncPath { get; }

        public string GetPath()
        {
            return IsFile ? File.FullName : Directory.FullName;
        }
    }
}