using System.Collections.Generic;

namespace XWidget.FFMpeg {
    public class VideoOption {
        public static class Preset {
            public const string ultrafast = "ultrafast";
            public const string superfast = "superfast";
            public const string veryfast = "veryfast";
            public const string faster = "faster";
            public const string fast = "fast";
            public const string medium = "medium";
            public const string slow = "slow";
            public const string slower = "slower";
            public const string veryslow = "veryslow";
            public const string placebo = "placebo";
        }


        internal Dictionary<string, string> args = new Dictionary<string, string>();

        public VideoOption SetBitrate(uint bitrate) {
            args["vb"] = bitrate.ToString();
            return this;
        }
        public VideoOption SetBitrate(uint min, uint max) {
            if (args.ContainsKey("vb")) {
                args.Remove("vb");
            }
            args["minrate"] = min.ToString();
            args["maxrate"] = max.ToString();
            return this;
        }
        public VideoOption SetCrf(uint crf) {
            args["crf"] = crf.ToString();
            return this;
        }

        public VideoOption SetPreset(string preset) {
            args["preset"] = preset;
            return this;
        }

        public VideoOption SetFPS(uint fps) {
            args["r"] = fps.ToString();
            return this;
        }

        public VideoOption SetSize(uint width, uint height) {
            args["s"] = $"{width}x{height}";
            return this;
        }

        public VideoOption Codec(string codec) {
            args["vcodec"] = codec;
            return this;
        }

        public VideoOption Copy() {
            return Codec("copy");
        }

        public VideoOption RemoveVideo() {
            args.Clear();
            args["vn"] = "";
            return this;
        }
    }
}