using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;

namespace QuickCopy.Utilities
{
    public class FileSystemEnumerable : IEnumerable<FileSystemInfo>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly SearchOption _option;
        private readonly IList<string> _patterns;

        private readonly DirectoryInfo _root;

        public FileSystemEnumerable(DirectoryInfo root, string pattern, SearchOption option)
        {
            _root = root;
            _patterns = new List<string> {pattern};
            _option = option;
        }

        public FileSystemEnumerable(DirectoryInfo root, IList<string> patterns, SearchOption option)
        {
            _root = root;
            _patterns = patterns;
            _option = option;
        }

        public IEnumerator<FileSystemInfo> GetEnumerator()
        {
            Logger.Debug("Getting enumerator");
            if (_root == null || !_root.Exists) yield break;

            IEnumerable<FileSystemInfo> matches = new List<FileSystemInfo>();
            try
            {
                Logger.Debug("Attempting to enumerate '{0}'", _root.FullName);
                foreach (var pattern in _patterns)
                {
                    Logger.Debug("Using pattern '{0}'", pattern);
                    matches = matches.Concat(_root.EnumerateDirectories(pattern, SearchOption.TopDirectoryOnly))
                        .Concat(_root.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly));
                }
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Warn("Unable to access '{0}'. Skipping...", _root.FullName);
                yield break;
            }
            catch (PathTooLongException ptle)
            {
                Debug.Assert(_root.Parent != null, "_root.Parent != null");
                Logger.Warn($@"Could not process path '{_root.Parent.FullName}\{_root.Name}'.",
                    ptle);
                yield break;
            }
            catch (IOException e)
            {
                Debug.Assert(_root.Parent != null, "_root.Parent != null");
                Logger.Warn(
                    $@"Could not process path (check SymlinkEvaluation rules)'{_root.Parent.FullName}\{_root.Name}'.",
                    e);
                yield break;
            }


            Logger.Debug("Returning all objects that match the pattern(s) '{0}'", string.Join(",", _patterns));
            foreach (var file in matches) yield return file;

            if (_option != SearchOption.AllDirectories) yield break;

            Logger.Debug("Enumerating all child directories.");
            foreach (var dir in _root.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                Logger.Debug("Enumerating '{0}'", dir.FullName);
                var fileSystemInfos = new FileSystemEnumerable(dir, _patterns, _option);
                foreach (var match in fileSystemInfos) yield return match;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}