using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pcc
{
    public class Tokenizer
    {
        private readonly string[] openedFileData;
        public Dictionary<string, string> tokenValues;
        public Dictionary<string, string> tokenCodeTable;

        public Tokenizer(string[] inputDatastream)
        {
            openedFileData = inputDatastream;
            // Listener.OnSusDetected += ExecuteTheSussy;
            BuildTokenCodeTable();
            tokenValues = new();
            CreateTokenDict();
        }

        private void BuildTokenCodeTable()
        {
            // This builds the token code table, literally just for ease.
            // I can just add a new token by using the name and a number after. This function just converts the integer after into a relevant token type.
            tokenCodeTable = new();
            string[] unpackedValues = File.ReadAllLines("TokenCodes.txt");
            foreach (string line in unpackedValues)
            {
                string[] tokenCodeSplit = line.Split(" ");
                tokenCodeTable.TryAdd(tokenCodeSplit[0], tokenCodeSplit[1]);
            }
            //List<string> keyList = new(this.tokenCodeTable.Keys);
            //Console.WriteLine("### Token Code key list: " + String.Join("|", keyList.ToArray()) + " ###");
            //List<string> valueList = new(this.tokenCodeTable.Values);
            //Console.WriteLine("### Token Code value list: " + String.Join("|", valueList.ToArray()) + " ###");
        }

        public List<List<(string, string)>> CreateTokenStream()
        {
            List<List<(string, string)>> tokens = new();
            foreach (string line in openedFileData)
            {
                string temp = line.Trim();
                if (temp.Contains('"'))
                {
                    temp = StringIdentifier(temp, '"');
                }
                tokens.Add(DecipherToTokens(temp.Split(" ")));
            }
            return tokens;
        }

        // Method to convert a line into a valid token stream. Hence the name "decipher".
        private List<(string, string)> DecipherToTokens(string[] toDecipher)
        {
            List<(string, string)> finalToReturn = new();
            foreach (string word in toDecipher)
            {
                try
                {
                    // Find each word in the token value dict that it corresponds to and create a tuple of (token, value)
                    (string, string) temp = (tokenValues[word], word);                    
                    finalToReturn.Add(temp);
                }
                catch (KeyNotFoundException)
                {
                    // This is the resolution of the unknown, non-reserved keywords.
                    // I.e, these are the keywords that are not the TokenValues.
                    try
                    {
                        // Try to convert the word into a float, if doesnt work then you know its either a variable or boolean or string
                        // Ignoring booleans for now, the parsed word must either be a string or float.
                        float.Parse(word);
                        finalToReturn.Add(new("float", float.Parse(word).ToString()));

                    }
                    catch (FormatException)
                    {
                        finalToReturn.Add(new("variable", word));
                    }
                }
            }
            // We need to consider the case of string literals with the identifier § before it.
            for (int n = 0; n < finalToReturn.Count; n++)
            {
                // Only process the items that are potential variables
                if (finalToReturn[n].Item1 == "variable")
                {
                    // Only consider the with § before it. Therefore, § is a banned keyword in the code.
                    if (finalToReturn[n].Item2[0] == '§')
                    {
                        // Get the number in between the pointer and the " that indicates the number of elements in the string.
                        int startIndex = finalToReturn[n].Item2.IndexOf('§');
                        int endIndex = finalToReturn[n].Item2.IndexOf('\"');
                        // Counter of how many elements
                        int counter = Int32.Parse(finalToReturn[n].Item2[(startIndex + 1)..endIndex]);
                        string temp = "";
                        int endToRemove = 0;
                        for (int i = 0; i < counter; i++)
                        {
                            // Add the elements to the temporary array
                            temp += finalToReturn[n + i].Item2;
                            // Add 1 to the index that needs to be removed.
                            endToRemove += 1;
                        }
                        //Console.WriteLine(n + endToRemove.ToString());
                        //Console.WriteLine("literal: " + temp);
                        // Remove the potential variables from the final list and replace it with the identified literal string
                        // While also stripping the indicator and element number.
                        finalToReturn.RemoveRange(n, endToRemove);
                        temp = temp.Replace(counter.ToString(), "");
                        temp = temp.Replace("§", "");
                        finalToReturn.Insert(n, new("string", temp));
                    }
                }
            }
            return finalToReturn;
        }

        private string StringIdentifier(string input, char toIdentify)
        {
            string toReturn = "";
            int startIndex = input.IndexOf(toIdentify);
            int endIndex = input.LastIndexOf(toIdentify) + 1;
            // Create a temporary variable with the extracted string.
            string temp = input[startIndex..endIndex];
            // Append a special character + length of string, to indicate how many spaced elements are in the string, to the temporary variable
            temp = "§" + temp.Split(" ").Length.ToString() + temp;
            toReturn = input[0..startIndex] + temp;
            return toReturn;
        }

        // Method to create the token dictionary, mapping each reserved keyword to its token type.
        private void CreateTokenDict()
        {
            // MAKE SURE THAT THIS DOES NOT CONTAIN ANY NULL OR WHITE SPACING AT THE END OF THE FILE
            string[] unpackedValues = File.ReadAllLines("TokenValues.txt");
            foreach (string line in unpackedValues)
            {
                // Done as a list so that values can be added this "temp".
                string[] splitLine = line.Split(" ");
                // Trying to reassign the integer into a relevant token type from the code dictionary.
                splitLine[1] = tokenCodeTable[splitLine[1]];
                //Console.WriteLine("+++" + String.Join("+", splitLine));
                try
                {
                    tokenValues[splitLine[0]] = splitLine[1];
                }
                catch (KeyNotFoundException)
                {
                    tokenValues.Add(splitLine[0], splitLine[1]);
                }
            }
        }

        private void ExecuteTheSussy(object sender, Listener.ListenEventDispatcher listened)
        {
            Console.WriteLine("Reached");
            if (listened.containsSusTrueorFalse == true)
            {
                Console.WriteLine("ES SUSSY");
            }
        }
    }
}
