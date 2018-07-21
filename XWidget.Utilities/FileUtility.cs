using System;
using System.IO;

namespace XWidget.Utilities {
    /// <summary>
    /// 檔案常用方法
    /// </summary>
    public static class FileUtility {
        /// <summary>
        /// 擦除內容後刪除檔案
        /// </summary>
        /// <param name="path">檔案路徑</param>
        /// <param name="overWriteTime">內容擦除次數，預設為一次</param>
        public static void SecureDelete(string path, uint overWriteTime = 1) {
            if (overWriteTime > 0) {
                //write 0x00
                WriteToFile(path, () => 0x00);

                //write 0xff
                WriteToFile(path, () => 0xff);

                Random rand = new Random((int)DateTime.Now.Ticks);

                //write 0xrand
                WriteToFile(path, () => {
                    byte[] data = new byte[] { 0x00 };
                    rand.NextBytes(data);
                    return data[0];
                });
            }
            if (overWriteTime == 0) {
                System.IO.File.Delete(path);
            } else {//>=1
                SecureDelete(path, overWriteTime - 1);
            }
        }

        private static void WriteToFile(string path, Func<byte> writeAction) {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write);
            long fileSize = fileStream.Length;
            BinaryWriter writer = new BinaryWriter(fileStream);
            for (long i = 0; i < fileSize; i++) {
                writer.Write(writeAction());
                if (i % 1024 == 0 && i > 0) writer.Flush();
            }
            writer.Flush();
            writer.Dispose();
            fileStream.Dispose();
        }
    }
}
