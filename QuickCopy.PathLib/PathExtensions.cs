using System.Diagnostics;
using System.IO;

namespace QuickCopy.PathLib
{
    public static class PathExtensions
    {
        public static bool IsUnc(this string path)
        {
            var root = Path.GetPathRoot(path);

            Debug.Assert(root != null, nameof(root) + " != null");
            if (root.StartsWith(@"\\"))
                return true;

            var drive = new DriveInfo(root);
            return drive.DriveType == DriveType.Network;
        }
    }
}
