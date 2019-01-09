using System;
using Xunit;

namespace XWidget.FFMpeg.Test {
    public class UnitTest1 {
        [Fact]
        public void Test1() {
            var builder = new FFMpegConverterBuilder();
            builder
                .ConfigGeneric(option =>
                    option.SetStartPosition(40).SetDuration(10)
                )
                .ConfigVideo(option =>
                    option.SetSize(352, 240).SetPreset(VideoOption.Preset.veryfast).SetCrf(23)
                )
                .ConfigAudio(option =>
                    option.SetChannels(1).SetFrequency(16 * 1000)
                );

            var kk = builder.BuildArgs();
        }
    }
}
