using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace pcc
{
    class LexicalAnalyzer : Program
    {
        public LexicalAnalyzer(string[] inputArgs)
        {
            parser = new(inputArgs);
            parser.CheckInputs();
            rawData = parser.GetInputFileData();
            tokeniser = new(rawData);
            rawTokenStream = tokeniser.CreateTokenStream();
        }

        // A method that simply flattens the raw token stream which is a 2D list of tuples into a 1D list of tuples for easier processing.
        private static List<(string, string)> FlattenTokenStream(List<List<(string, string)>> toFlatten)
        {
            List<(string, string)> flattenedStream = new();
            foreach (List<(string, string)> row in toFlatten)
            {
                foreach ((string, string) column in row)
                {
                    flattenedStream.Add(column);
                }
            }
            return flattenedStream;
        }

        public List<(string, string, string, string, string)> CreateSymbolTable()
        {
            // This is why the flattener is needed.
            // Makes the symbol table creation infintely easier and arguably faster.
            tokenStream = FlattenTokenStream(rawTokenStream);

            // The following attributes are stored in the symbol table:
            // Lexeme, Type, Value, Class, Scope
            List<(string, string, string, string, string)> table = new();

            foreach ((string, string) token in tokenStream)
            {
                if (token.Item1 == "variable")
                {
                    table.Add((token.Item2, DiscoverTokenType(token), DiscoverTokenValue(token, ("assignment", "="), 1), DiscoverTokenClass(token), DiscoverTokenScope(token)));
                }
            }
            return table;
        }

        private string DiscoverTokenValue((string, string) tokenToDiscover, (string, string) tokenComparator, int n)
        {
            int index = tokenStream.IndexOf(tokenToDiscover);
            try
            {
                if (tokenStream[index + n] == tokenComparator)
                {
                    return tokenStream[index + (n + 1)].Item2;
                }
            }
            // TTD: CATCH PRINT TO DEBUG LOG
            catch { };
            // RESOLVE FOR CLASSES BUT NOT NOW
            return "nil";
        }

        private string DiscoverTokenClass((string, string) tokenToDiscover)
        {
            // Implement classes later, for now, everything is global
            //int index = tokenStream.IndexOf(tokenToDiscover);
            //List<(string, string)> before = new();
            //for (int i = 0; i < index; i++)
            //{
            //    before.Add(tokenStream[i]);
            //}
            //Predicate<(string, string)> condition = MatchToken;
            //List<(string, string)> temp = before.FindAll(condition);
            //if (temp.Count == 0)
            //{
            //    return "global";
            //}
            // return DiscoverTokenValue(tokenToDiscover, tokenStream[tokenStream.IndexOf(temp[^1])], 0);
            if (tokenToDiscover.Item1 == "variable")
            {
                return "global";
            }
            // No Valid Class
            return "nvc";
        }

        private static bool MatchToken((string, string) input)
        {
            return input.Item1 == "class";
        }

        private string DiscoverTokenScope((string, string) tokenToDiscover)
        {
            // Implement scope later, for now everything is also globally scoped.
            if (tokenToDiscover.Item1 == "variable")
            {
                return "global";
            }
            // No Valid Scope
            return "nvs";
        }

        private string DiscoverTokenType((string, string) tokenToDiscover)
        {
            int index = tokenStream.IndexOf(tokenToDiscover);
            try
            {
                if (tokenStream[index - 1].Item1 == "type")
                {
                    return tokenStream[index - 1].Item2;
                }
            }
            catch { };
            return "var";
        }
    }
}
