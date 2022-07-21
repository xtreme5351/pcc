using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace pcc
{
    class SemanticAnalyzer : Program
    {
        public SemanticAnalyzer()
        {
            this.Test();
        }

        public static string SymbolTableTypeLookup(string item)
        {
            string final = "~";
            foreach ((string, string, string, string, string) record in symbolTable)
            {
                if (record.Item1 == item)
                {
                    return record.Item2;
                }
            }
            return final;
        }

        public static ParseNode BuildAssignmentTree(List<(string, string)> line, int level, int variableN)
        {
            // BUILT TO TEST IDEA, NOT ACTUAL AS DOESNT WORK FOR let x = 1 + 2 + 3 or more arithmetic
            ParseNode root = new($"Assignment", "Assignment", "=");
            string variableName = rawTokenStream[level][variableN].Item2;
            root.AssignLeftNode(new ParseNode("Variable", SymbolTableTypeLookup(variableName), variableName));
            //Console.WriteLine(root.left.value + root.left.type);

            Regex regex = new(@"[\/\+\-\*]");
            Match match = regex.Match(rawData[level]);
            string operatorIndex = match.Value;
            try
            {
                ParseNode operatorNode = new("Operator", "Arithmetic", line[Int32.Parse(operatorIndex)].Item2);
                operatorNode.AssignLeftNode(new ParseNode("Value", root.type, line[Int32.Parse(operatorIndex) - 1].Item2));
                operatorNode.AssignRightNode(new ParseNode("Value", root.type, line[Int32.Parse(operatorIndex) + 1].Item2));
                root.AssignRightNode(operatorNode);
            }
            catch
            {
                root.AssignRightNode(new ParseNode("Value", root.type, line[variableN + 2].Item2));
            }
            return root;
        }

        public void TraverseTreeInorder(ParseNode Root)
        {
            if (Root != null)
            {
                TraverseTreeInorder(Root.left);
                Console.WriteLine(Root.value + " ");
                TraverseTreeInorder(Root.right);
            }
        }

        public void Test()
        {
            //foreach ((string, string, string, string, string) record in symbolTable)
            //{
            //    Console.WriteLine(record);
            //}
            foreach(List<(string, string)> line in rawTokenStream)
            {
                int lineLevel = rawTokenStream.IndexOf(line);
                int n = 0;
                foreach ((string, string) token in line)
                {
                    if (token.Item1 == "variable")
                    {
                        n = line.IndexOf(token);
                        break;
                    }
                }
                ParseNode root = BuildAssignmentTree(line, lineLevel, n);
                TraverseTreeInorder(root);
            }
        }
    }
}
