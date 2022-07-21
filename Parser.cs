using System;
using System.Collections.Generic;
using System.IO;

namespace pcc
{
    public class Parser
    {
        private readonly string[] inputArgs;
        private Dictionary<string, string> argsDict;
        private static readonly MissingFieldException missingFileError = new("=== Input or output file paths missing ===");
        private static readonly ArgumentException invalidFileError = new("=== File not valid ===");

        public Parser(string[] args)
        {
            inputArgs = args;
            argsDict = new();
        }

        public void CheckInputs()
        {
            try
            {
                argsDict.Add("inputPath", inputArgs[0]);
            }
            catch (IndexOutOfRangeException)
            {
                throw missingFileError;
            }

            //try
            //{
            //    argsDict.Add("outputPath", inputArgs[1]);
            //}
            //catch (IndexOutOfRangeException)
            //{
            //    throw missingFileError;
            //}

            //if (!Directory.Exists(argsDict["inputPath"]))
            //{
            //    throw invalidFileError;
            //}

            // Console.WriteLine(Path.GetFullPath(inputArgs[0]));
            if (Path.GetExtension(Path.GetFullPath(inputArgs[0])) != ".pvc")
            {
                throw new Exception("Incorrect input file type");
            }
        }

        public string[] GetInputFileData()
        {
            return File.ReadAllLines(inputArgs[0]);
        }
    }
}
