using System;
using System.Linq;

namespace XWidget.FFMpeg {
    public class FFMpegConverterBuilder {
        private GenericOption generic = new GenericOption();
        private VideoOption video = new VideoOption();
        private AudioOption audio = new AudioOption();
        private string exePath = "ffmpeg";
        private string advancedArgs = string.Empty;

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

        private string CreateCommand() {
            var generic_str = string.Join(" ", generic.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));
            var video_str = string.Join(" ", video.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));
            var audio_str = string.Join(" ", audio.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));

            var command = string.Join(" ", generic_str, video_str, audio_str, advancedArgs);

            return command;
            //return string.Join(" ", command, string.Join(" ", inputs.Select(x => "-i " + x)), output);
        }

        public FFMpegConverter Build() {
            return new FFMpegConverter(exePath, CreateCommand());
        }
    }
}
