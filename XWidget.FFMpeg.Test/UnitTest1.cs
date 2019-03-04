using System;
using Xunit;

namespace XWidget.FFMpeg.Test {
    public class UnitTest1 {
        [Fact]
        public void Test1() {
            var builder = new FFMpegConverterBuilder();
            builder
                .ConfigGeneric(option => //跳過40秒取10秒
                    option.SetStartPosition(40).SetDuration(10)
                )
                .ConfigVideo(option => //設定新尺寸且轉換速度設為非常快，調整品質為23
                    option.SetSize(352, 240).SetPreset(VideoOption.Preset.veryfast).SetCrf(23)
                )
                .ConfigAudio(option => //設定聲音取樣率為16K且為單聲道，比特率為32K
                    option.SetChannels(1).SetFrequency(16 * 1000).SetBitrate(32 * 1000)
                );

            //var command = builder.CreateCommand(new string[] { "input.mp4" }, "output.mp4");
        }

        [Fact]
        public void Test2() {
            var builder = new FFMpegConverterBuilder();
            builder
                .ConfigGeneric(option => //跳過40秒取10秒
                    option.SetStartPosition(40).SetDuration(10)
                )
                .ConfigVideo(option => //去除影像
                    option.RemoveVideo()
                )
                .ConfigAudio(option => //設定聲音取樣率為16K且為單聲道，比特率為32K
                    option.SetChannels(1).SetFrequency(16 * 1000).SetBitrate(32 * 1000)
                );

            //var kk = builder.CreateCommand(new string[] { "input.mp4" }, "output.mp3");
        }
    }
}
