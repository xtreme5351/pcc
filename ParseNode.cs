using System;
namespace pcc
{
    public class ParseNode
    {
        public string name;
        public string type;
        public string value;
        public ParseNode left;
        public ParseNode right;
        //public ParseNode middle;

        public ParseNode(string NodeName, string NodeType, string NodeValue)
        {
            name = NodeName;
            type = NodeType;
            value = NodeValue;
        }

        public void AssignLeftNode(ParseNode node)
        {
            left = node;
        }

        public void AssignRightNode(ParseNode node)
        {
            right = node;
        }

        //public void AssignMiddleNode(ParseNode node)
        //{
        //    middle = node;
        //}
    }
}


