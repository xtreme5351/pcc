using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace pcc
{
    class SemanticAnalyzer : Program
    {
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

        // This is basically a struct.
        // Here is the implementation of my genius, the orchard.
        // Each node in the graph can have a list of other assignment trees and is called the orchard.
        // It is an optional child to each node.
        // In theory, you should be able to dispatch the traversal of the orchard to another process
        // e.g. to another core (paralell processing) as this is just memory assignment
        public static ParseNode BuildDataBucket(int level, int startLine)
        {
            ParseNode root = new("Databucket", "Databucket", "{}");
            string variableName = rawTokenStream[level][2].Item2;
            root.AssignLeftNode(new ParseNode("Variable", "foobar", variableName));
            List<ParseNode> dataItems = new();
            int endLine = 0;
            for (int i = startLine; i < rawTokenStream.Count; i++)
            {
                if (rawTokenStream[i][0].Equals(("separator", "}")))
                {
                    endLine = i;
                    break;
                }
            }
            List<List<(string, string)>> localScope = rawTokenStream.GetRange(startLine + 1, endLine - startLine - 1);
            foreach (List<(string, string)> assignmentLine in localScope)
            {
                int index = 0;
                foreach ((string, string) token in assignmentLine)
                {
                    if (token.Item1 == "variable")
                    {
                        index = assignmentLine.IndexOf(token);
                    }
                }
                dataItems.Add(BuildAssignmentTree(assignmentLine, rawTokenStream.IndexOf(assignmentLine), index));
            }
            root.orchard = dataItems;
            return root;
        }

        public static ParseNode BuildAssignmentTree(List<(string, string)> line, int level, int variableN)
        {
            // NOT FULLY WORKING AS DOESNT WORK FOR let x = 1 + 2 + 3 or more arithmetic
            ParseNode root = new("Assignment", "Assignment", "=");
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

        public static void TraverseTreeInorder(ParseNode Root)
        {
            if (Root != null)
            {
                TraverseTreeInorder(Root.left);
                if (Root.orchard != null)
                {
                    foreach (ParseNode node in Root.orchard)
                    {
                        TraverseTreeInorder(node);
                    }
                }
                else
                {
                    Console.WriteLine(Root.value + " ");
                }
                TraverseTreeInorder(Root.right);
            }
        }

        public List<ParseNode> BuildParseGraph()
        {
            List<ParseNode> ParseGraph = new();
            foreach(List<(string, string)> line in rawTokenStream)
            {
                int lineLevel = rawTokenStream.IndexOf(line);
                foreach ((string, string) token in line)
                {
                    int index = line.IndexOf(token);
                    switch (token.Item1)
                    {
                        case "variable":
                            ParseGraph.Add(BuildAssignmentTree(line, lineLevel, index));
                            break;

                        case "databucket":
                            BuildDataBucket(lineLevel, rawTokenStream.IndexOf(line));
                            break;

                        default:
                            break;
                    }
                }
            }
            return ParseGraph;
        }
    }
}
