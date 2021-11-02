using System;

namespace TSI.FourSeries.FileOperations
{

    public class FileContentsReadSuccessfullyEventArgs : EventArgs
    {
        public string FileContentsArg { get; set; }
    }

    public class DeserializationSuccessEventArgs : EventArgs
    {
        public string LastUpdate { get; set; }

        public string[] StringArray { get; set; }

        public ushort ListCount { get; set; }
    }

    public class FileWriteSuccessEventArgs : EventArgs
    {
        public string LastUpdate { get; set; }
    }

}
