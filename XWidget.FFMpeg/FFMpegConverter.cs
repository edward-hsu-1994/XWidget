using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XWidget.FFMpeg {
    public class FFMpegConverter {
        Dictionary<string, string> args;
        string exePath;
        internal FFMpegConverter(string exePath, Dictionary<string, string> args) {
            this.exePath = exePath;
            this.args = args;
        }

        private string CreateArgs() {
            return string.Join(" ", args.Select(x => $"-{x.Key} {x.Value ?? ""}"));
        }

        /// <summary>
        /// 轉換影片格式
        /// </summary>
        /// <param name="inputPaths">輸入路徑</param>
        /// <param name="outputPath">輸出路徑</param>
        /// <returns>轉換結果</returns>
        public IObservable<ConvertResult> Convert(string[] inputPaths, string outputPath) {
            var process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = string.Join(" ", string.Join(" ", inputPaths.Select(x => $"-i \"{x.Replace("\\", "/")}\"")), CreateArgs(), $"\"{outputPath.Replace("\\", "/")}\"");
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

        /// <summary>
        /// 轉換影片格式
        /// </summary>
        /// <param name="inputPath">輸入路徑</param>
        /// <param name="outputPath">輸出路徑</param>
        /// <returns>轉換結果</returns>
        public IObservable<ConvertResult> Convert(string inputPath, string outputPath) {
            return Convert(new string[] { inputPath }, outputPath);
        }

        /// <summary>
        /// 轉換影片格式
        /// </summary>
        /// <param name="inputPaths">輸入路徑</param>
        /// <param name="outputPath">輸出路徑</param>
        /// <returns>轉換結果</returns>
        public Task<ConvertResult> ConvertAsync(string[] inputPaths, string outputPath) {
            var source = new TaskCompletionSource<ConvertResult>();

            Convert(inputPaths, outputPath).Subscribe(x => {
                source.SetResult(x);
            });

            return source.Task;
        }

        /// <summary>
        /// 轉換影片格式
        /// </summary>
        /// <param name="inputPath">輸入路徑</param>
        /// <param name="outputPath">輸出路徑</param>
        /// <returns>轉換結果</returns>
        public Task<ConvertResult> ConvertAsync(string inputPath, string outputPath) {
            return ConvertAsync(new string[] { inputPath }, outputPath);
        }

        /// <summary>
        /// 取得影片截圖
        /// </summary>
        /// <param name="inputPath">輸入路徑</param>
        /// <param name="sec">秒數</param>
        /// <param name="format">格式</param>
        /// <returns>截圖串流</returns>
        public IObservable<Stream> GetThumbnail(
            string inputPath,
            int sec = 0,
            ImageFormat format = ImageFormat.JPEG) {
            var thumbnail = Path.GetTempFileName();
            switch (format) {
                case ImageFormat.JPEG:
                    thumbnail += ".jpg";
                    break;
                case ImageFormat.BMP:
                    thumbnail += ".bmp";
                    break;
                case ImageFormat.PNG:
                    thumbnail += ".png";
                    break;
            }

            int ss = args.ContainsKey("ss") ? int.Parse(args["ss"]) : 0;
            ss += sec;

            string size = string.Empty;
            if (args.ContainsKey("s")) {
                size = "-s " + args["s"];
            }

            var process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = string.Join(" ", $"-i \"{inputPath.Replace("\\", "/")}\"", $"-ss {ss} {size} -vframes 1", $"\"{thumbnail.Replace("\\", "/")}\"");
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


            var result = Observable.Create<Stream>(async (x) => {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                MemoryStream _r = null;

                try {
                    _r = new MemoryStream(File.ReadAllBytes(thumbnail));
                    _r.Seek(0, SeekOrigin.Begin);

                    File.Delete(thumbnail);
                } catch { }

                x.OnNext(_r);

                x.OnCompleted();
            });

            return result;
        }

        /// <summary>
        /// 取得影片截圖
        /// </summary>
        /// <param name="inputPath">輸入路徑</param>
        /// <param name="sec">秒數</param>
        /// <param name="format">格式</param>
        /// <returns>截圖串流</returns>
        public Task<Stream> GetThumbnailAsync(
            string inputPath,
            int sec = 0,
            ImageFormat format = ImageFormat.JPEG) {
            var source = new TaskCompletionSource<Stream>();

            GetThumbnail(inputPath, sec, format).Subscribe(x => {
                source.SetResult(x);
            });

            return source.Task;
        }
    }
}