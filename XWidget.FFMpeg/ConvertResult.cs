using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.FFMpeg {
    public class ConvertResult {
        public int ExitCode { get; internal set; }
        public string[] InputPaths { get; internal set; }
        public string OutputPath { get; internal set; }
        public TimeSpan ProcessorTime { get; internal set; }
        public string Log { get; internal set; }
    }
}
