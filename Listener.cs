using System;
using System.IO;

namespace pcc
{
    public class Listener
    {
        public static event EventHandler<ListenEventDispatcher> OnSusDetected;
        private static Parser parser;
        //private readonly string filePath;
        private readonly string[] openedFileData;

        public Listener(string inputPath)
        {
            parser = new(new string[] { inputPath });
            openedFileData = parser.GetInputFileData();
        }

        public class ListenEventDispatcher
        {
            public bool containsSusTrueorFalse;
        }

        public void DispatchSusEvent()
        {
            OnSusDetected?.Invoke(this, new ListenEventDispatcher
            {
                containsSusTrueorFalse = this.ListenForSussy()
            });
        }

        private bool ListenForSussy()
        {
            foreach (string line in openedFileData)
            {
                Console.WriteLine("REACHED 2");
                if (line.IndexOf("AMOGUS") != -1)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
