namespace RCSHTools {
    internal interface ISolveTreeNode
    {
        string Value {get; set;}
        ISolveTreeNode[] Children {get; set;}
        ISolveTreeNode Parent {get; set;}
        double DoAction();
    }

    internal class SolveTreeOperatorNode : ISolveTreeNode {
        public string Value {get; set;}
        public ISolveTreeNode[] Children {get; set;}
        public ISolveTreeNode Parent {get; set;}

        public ISolveTreeNode Left {
            get => Children[0];
            set => Children[0] = value;
        }
        public ISolveTreeNode Right {
            get => Children[1];
            set => Children[1] = value;
        }

        public SolveTreeOperatorNode(string value){
            Value = value;
            Children = new ISolveTreeNode[2];
        }

        public double DoAction(){
            switch(Value){
                case "+":
                    return Left.DoAction() + Right.DoAction();
                case "-":
                    return Left.DoAction() - Right.DoAction();
                case "*":
                    return Left.DoAction() * Right.DoAction();
                case "/":
                    return Left.DoAction() / Right.DoAction();
            }
            throw new System.Exception("Unhandled operator");
        }
    }

    internal class SolveTreeValueNode : ISolveTreeNode {
        public string Value {get; set;}
        public ISolveTreeNode[] Children {get; set;}
        public ISolveTreeNode Parent {get; set;}

        public SolveTreeValueNode(string value, ISolveTreeNode parent){
            Value = value;
            Children = new ISolveTreeNode[0];
            Parent = parent;
        }

        public double DoAction() {
            return double.Parse(Value);
        }
    }
}