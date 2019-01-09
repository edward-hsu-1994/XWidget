using System;
using System.Linq;

namespace XWidget.FFMpeg {
    public class FFMpegConverterBuilder {
        private GenericOption generic = new GenericOption();
        private VideoOption video = new VideoOption();
        private AudioOption audio = new AudioOption();

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

        public string BuildArgs() {
            var generic_str = string.Join(" ", generic.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));
            var video_str = string.Join(" ", video.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));
            var audio_str = string.Join(" ", audio.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));

            return string.Join(" ", generic_str, video_str, audio_str);
        }

        public IFFMpegConverter Build() {
            var generic_str = string.Join(" ", generic.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));
            var video_str = string.Join(" ", video.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));
            var audio_str = string.Join(" ", audio.args.Select(x => $"-{x.Key} {x.Value ?? ""}"));



            return null;
        }
    }
}
