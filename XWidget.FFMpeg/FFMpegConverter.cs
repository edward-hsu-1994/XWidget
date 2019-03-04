using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace XWidget.FFMpeg {
    public class FFMpegConverter {
        string args = string.Empty;
        string exePath = string.Empty;
        internal FFMpegConverter(string exePath, string args) {
            this.exePath = exePath;
            this.args = args;
        }

        public IObservable<ConvertResult> Convert(string[] inputPaths, string outputPath) {
            var process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = string.Join(" ", string.Join(" ", inputPaths.Select(x => $"-i \"{x.Replace("\\", "/")}\"")), args, $"\"{outputPath.Replace("\\", "/")}\"");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            string g = "";
            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e) {
                g += e.Data + Environment.NewLine;
            };
            process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e) {
                g += e.Data + Environment.NewLine;
            };


            var result = Observable.Create<ConvertResult>(async (x) => {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                x.OnNext(new ConvertResult() {
                    ExitCode = process.ExitCode,
                    InputPaths = inputPaths,
                    OutputPath = outputPath,
                    ProcessorTime = process.TotalProcessorTime,
                    Log = g
                });
                x.OnCompleted();
            });

            return result;
        }
    }
}