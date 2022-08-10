using System;
using System.Collections.Generic;

namespace pcc
{
    class Program
    {
        private static LexicalAnalyzer lexicalAnalysis;
        private static SemanticAnalyzer semantic;

        protected static Parser parser;
        protected static Tokenizer tokeniser;
        protected static string[] rawData;
        protected static List<List<(string, string)>> rawTokenStream;
        protected static List<(string, string)> tokenStream;
        protected static List<(string, string, string, string, string)> symbolTable;
        protected static List<ParseNode> executionOrder;

        static void Main(string[] args)
        {
            // ALL OF THE METHODS IN EVERY CLASS NEEDS TO BE OPTIMISED, ITS DOGSHIT OPTIMISATION ATM.
            lexicalAnalysis = new(args);
            symbolTable = lexicalAnalysis.CreateSymbolTable();
            Console.WriteLine(String.Join(" ", tokenStream));
            semantic = new();
            executionOrder = semantic.BuildParseGraph();
        }
    }
}
