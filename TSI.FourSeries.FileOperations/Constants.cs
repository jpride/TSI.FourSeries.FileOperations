using System;


namespace TSI.FourSeries.FileOperations
{
    class Constants
    {
        internal static string FileReadErrorMessage = "File Read Error: {0}";
        internal static string UpdateListErrorMessage = "Update List Error @ Index: {0}:  {1}";
        internal static string DeserializeErrorMessage = "Deserialization Error: {0}";
        internal static string DeserializationEventErrorMessage = "Deserialization Event Error: {0}";
        internal static string WriteFileErrorMessage = "File Write Error: {0}";
        internal static string SerializeObjectErrorMessage = "Serialize Object Error: {0}";
        internal static string FileDoesNotExistMessage = "File does not exist.";
        internal static string FileContentssMessage = "File Contents: {0}";
        internal static string DefaultFilePath = "/User/StringArray.json";
        internal static string EnteringCriticalSectionMessage = "Entering Critical Section";
        internal static string LeavingCriticalSectionMessage = "Leaving Critical Section";
    }
}
