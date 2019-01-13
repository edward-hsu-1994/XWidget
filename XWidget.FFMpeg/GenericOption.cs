using System.Collections.Generic;

namespace XWidget.FFMpeg {
    public class GenericOption {
        internal Dictionary<string, string> args = new Dictionary<string, string>();

        public GenericOption SetStartPosition(uint sec) {
            args["ss"] = sec.ToString();

            return this;
        }
        public GenericOption SetDuration(uint sec) {
            args["t"] = sec.ToString();

            return this;
        }

        public GenericOption SetTitle(string title) {
            args["title"] = title;

            return this;
        }

        public GenericOption SetAuthor(string author) {
            args["author"] = author;

            return this;
        }

        public GenericOption SetCopyright(string copyright) {
            args["copyright"] = copyright;

            return this;
        }

        public GenericOption SetComment(string comment) {
            args["comment"] = comment;

            return this;
        }
    }
}