using System;
using System.Collections.Generic;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Newtonsoft.Json;

namespace TSI.FourSeries.FileOperations
{

    public class FileOperations
    {
        Debug debug = new Debug();

        private FileMode _fileMode = FileMode.Create;
        private string _filePath;
        private readonly CCriticalSection _fileLock = new CCriticalSection();

        private JsonObject _jsonObject = new JsonObject();

        private List<string> IncomingStringList = new List<string>();

        //EVENTS
        public event EventHandler<FileContentsReadSuccessfullyEventArgs> FileContentsReadSuccessfullyEvent;
        public event EventHandler<DeserializationSuccessEventArgs> DeserializationSuccessEvent;
        public event EventHandler<FileWriteSuccessEventArgs> FileWriteSuccessEvent;


        //Properties
        private string FileContents { get; set; }

        public string FilePath
        {
            get
            {
                if (_filePath.Equals(String.Empty) || _filePath.Equals(null))
                {
                    return Constants.DefaultFilePath;
                }
                else
                {
                    return _filePath;
                }
            }
            set
            {
                _filePath = value;
            }
        }

        //Methods
        public void SetDebugEnable(ushort enable)
        {
            debug.UshortDebugEnable = enable.Equals(1) ? (ushort)1 : (ushort)0;
        }

        public void ReadFile()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    _fileLock.Enter();
                    if (debug.BoolDebugEnable) CrestronConsole.PrintLine(Constants.EnteringCriticalSectionMessage);

                    FileContents = File.ReadToEnd(FilePath, Encoding.UTF8);

                    if (debug.BoolDebugEnable) CrestronConsole.PrintLine(Constants.FileContentssMessage, FileContents);

                    DeserializeContents(FileContents);


                    #region EventArg creation and EventHandler Call
                    FileContentsReadSuccessfullyEventArgs args = new FileContentsReadSuccessfullyEventArgs
                    {
                        FileContentsArg = !FileContents.Equals(null) ? FileContents : String.Empty
                    };

                    var handler = FileContentsReadSuccessfullyEvent;
                    if (handler != null)
                    {
                        handler(this, args);
                    }
                    #endregion
                }
                else
                {
                    CrestronConsole.PrintLine(Constants.FileDoesNotExistMessage);
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine(Constants.FileReadErrorMessage, ex.Message);
            }
            finally
            {
                _fileLock.Leave();
                if (debug.BoolDebugEnable) CrestronConsole.PrintLine(Constants.LeavingCriticalSectionMessage);
            }
        }

        public void WriteFile()
        {
            FileStream stream = new FileStream(FilePath, _fileMode);

            try
            {
                var payload = SerializeData(IncomingStringList);
                _fileLock.Enter();
                stream.Write(payload, Encoding.UTF8);

                FileWriteSuccessEventArgs args = new FileWriteSuccessEventArgs
                {
                    LastUpdate = _jsonObject.LastUpdated
                };

                if (!FileWriteSuccessEvent.Equals(null))
                {
                    FileWriteSuccessEvent(this, args);
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine(Constants.WriteFileErrorMessage, ex.Message);
            }
            finally
            {
                _fileLock.Leave();
                stream.Close();
            }
        }

        public void UpdateListFromSimpl(ushort Index, string Item)
        {
            if (debug.BoolDebugEnable) CrestronConsole.PrintLine("[UpdateListFromSimpl] Index: {0} | incomingStringList.Count: {1}", Index, IncomingStringList.Count);

            try
            {
                if (Index < IncomingStringList.Count)
                {
                    IncomingStringList[Index] = Item;
                }
                else
                {
                    IncomingStringList.Add(Item);
                }

                if (debug.BoolDebugEnable) CrestronConsole.PrintLine("[UpdateListFromSimpl] incomingStringList[{0}]: {1}", Index, IncomingStringList[Index] ?? "Empty");
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine(Constants.UpdateListErrorMessage, Index, ex.Message);
            }

        }

        private string SerializeData(List<string> incomingList)
        {
            try
            {
                _jsonObject.LastUpdated = DateTime.Now.ToString();

                #region debugging
                if (debug.BoolDebugEnable) CrestronConsole.PrintLine("SerializeData | LastUpdated: {0}", _jsonObject.LastUpdated);

                int i = 0;
                if (debug.BoolDebugEnable) CrestronConsole.PrintLine("SerializeData | incomingList:");
                foreach (string item in incomingList)
                {
                    if (debug.BoolDebugEnable) CrestronConsole.PrintLine("SerializeData | incomingList{0}: {1}", i, item);
                    i++;
                }
                #endregion

                _jsonObject.Strings = incomingList;

                if (debug.BoolDebugEnable) CrestronConsole.PrintLine("SerializeData Result: {0}", JsonConvert.SerializeObject(_jsonObject));
                return JsonConvert.SerializeObject(_jsonObject);
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine(Constants.SerializeObjectErrorMessage, ex.Message);
                return String.Empty;
            }
        }

        private void DeserializeContents(string data)
        {
            try
            {
                _jsonObject = JsonConvert.DeserializeObject<JsonObject>(data);
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine(Constants.DeserializeErrorMessage, ex.Message);
            }

            #region EventArg creation and EventHandler Call
            try
            {
                DeserializationSuccessEventArgs args = new DeserializationSuccessEventArgs
                {
                    LastUpdate = _jsonObject.LastUpdated,
                    StringArray = _jsonObject.Strings.ToArray(),
                    ListCount = (ushort)_jsonObject.Strings.Count
                };

                if (!DeserializationSuccessEvent.Equals(null))
                {
                    DeserializationSuccessEvent(this, args);
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine(Constants.DeserializationEventErrorMessage, ex.Message);
            }
            #endregion

        }

    }
}
