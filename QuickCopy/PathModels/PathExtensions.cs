using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace QuickCopy.PathModels
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
