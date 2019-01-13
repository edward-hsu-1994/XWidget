using System.Collections.Generic;
namespace XWidget.FFMpeg {
    public class AudioOption {
        internal Dictionary<string, string> args = new Dictionary<string, string>();

        public AudioOption SetBitrate(uint bitrate) {
            args["ab"] = bitrate.ToString();
            return this;
        }

        public AudioOption SetFrequency(uint frequency) {
            args["ar"] = frequency.ToString();
            return this;
        }

        public AudioOption SetChannels(uint channels) {
            args["ac"] = channels.ToString();
            return this;
        }

        public AudioOption Codec(string codec) {
            args["acodec"] = codec;
            return this;
        }

        public AudioOption Copy() {
            args.Clear();
            return Codec("copy");
        }

        public AudioOption RemoveAudio() {
            args.Clear();
            args["an"] = "";
            return this;
        }
    }
}