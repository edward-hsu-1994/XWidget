using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace XWidget.FFMpeg {
    public class FFMpegConverterBuilder {
        private GenericOption generic = new GenericOption();
        private VideoOption video = new VideoOption();
        private AudioOption audio = new AudioOption();
        private string exePath;
        private string advancedArgs = string.Empty;

        public FFMpegConverterBuilder() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                exePath = "ffmpeg.exe";
            } else {
                exePath = "ffmpeg";
            }
        }

        public FFMpegConverterBuilder SetExecutePath(string exePath) {
            this.exePath = exePath;
            return this;
        }

        public FFMpegConverterBuilder ConfigGeneric(Action<GenericOption> genericConfig) {
            genericConfig(generic);
            return this;
        }

        public FFMpegConverterBuilder ConfigVideo(Action<VideoOption> videoConfig) {
            videoConfig(video);
            return this;
        }

        public FFMpegConverterBuilder ConfigAudio(Action<AudioOption> audioConfig) {
            audioConfig(audio);
            return this;
        }

        public FFMpegConverterBuilder ConfigAdvancedArgs(string args) {
            advancedArgs = args;
            return this;
        }

        private Dictionary<string, string> MergeArgs() {
            Dictionary<string, string> merge = new Dictionary<string, string>();

            foreach (var kv in generic.args) {
                merge[kv.Key] = kv.Value;
            }

            foreach (var kv in video.args) {
                merge[kv.Key] = kv.Value;
            }

            foreach (var kv in audio.args) {
                merge[kv.Key] = kv.Value;
            }

            return merge;
            //return string.Join(" ", command, string.Join(" ", inputs.Select(x => "-i " + x)), output);
        }

        public FFMpegConverter Build() {
            return new FFMpegConverter(exePath, MergeArgs());
        }
    }
}
