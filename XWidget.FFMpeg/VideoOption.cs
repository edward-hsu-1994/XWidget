using System.Collections.Generic;

namespace XWidget.FFMpeg {
    public class VideoOption {
        public static class Preset {
            public const string UltraFast = "ultrafast";
            public const string SuperFast = "superfast";
            public const string VeryFast = "veryfast";
            public const string Faster = "faster";
            public const string Fast = "fast";
            public const string Medium = "medium";
            public const string Slow = "slow";
            public const string Slower = "slower";
            public const string Veryslow = "veryslow";
            public const string Placebo = "placebo";
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

        public VideoOption SetSize(CommonSize size) {
            switch (size) {
                case CommonSize.SD:
                    SetSize(720, 576);
                    break;
                case CommonSize.HD:
                    SetSize(1280, 720);
                    break;
                case CommonSize.FullHD:
                    SetSize(1920, 1080);
                    break;
                case CommonSize.UHD:
                    SetSize(3840, 2160);
                    break;
                case CommonSize.HV:
                    SetSize(4096, 2160);
                    break;
                case CommonSize.SHV:
                    SetSize(7680, 4320);
                    break;
            }
            return this;
        }

        public VideoOption Codec(string codec) {
            args["vcodec"] = codec;
            return this;
        }

        public VideoOption Copy() {
            args.Clear();
            return Codec("copy");
        }

        public VideoOption RemoveVideo() {
            args.Clear();
            args["vn"] = "";
            return this;
        }
    }
}