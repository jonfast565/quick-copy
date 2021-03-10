using System;
using System.Collections.Generic;
using System.Text;

namespace QuickCopy.PathLib
{
    public class FileInfoParserActionList
    {
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public List<FileInfoParserAction> Actions { get; set; }
    }
}
