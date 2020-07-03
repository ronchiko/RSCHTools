namespace RCSHTools
{
    /// <summary>
    /// A tree used to solve algbric equations (basic operations like +-/*())
    /// </summary>
    public class SolveTree {

        private static int GetPercende(char o){
            switch (o){
                case '+': case '-':
                    return 1;
                case '*': case '/':
                    return 2;
            }
            return 0;
        }

        private const string operators = "()+-*/";

        private ISolveTreeNode root;

        /// <summary>
        /// Creates a new solve tree
        /// </summary>
        /// <param name="eq"></param>
        public SolveTree(string eq){
            eq = eq.Replace(" ","");
            root = GenerateNode(eq, null);
        }

        /// <summary>
        /// Solves the 
        /// </summary>
        /// <returns></returns>
        public double Solve(){
            try {
                return root.DoAction();
            }catch {
                throw new System.Exception("Failed solving expression");
            }
        }

        
        private ISolveTreeNode GenerateNode(string eq, ISolveTreeNode parent){

            if(eq.StartsWith("(")){
                int lvl = 1, i = 1;
                while(lvl > 0 && i < eq.Length){
                    if(eq[i] == '(')lvl++;
                    if(eq[i] == ')')lvl--;
                    i++;
                }
                //System.Console.WriteLine("--{0}/{1}",i,eq.Length - 1);
                if(i == eq.Length){
                    eq = eq.Substring(1, eq.Length - 2);
                }
            }
            
            //Find level 1 equations
            int level = 0, correctionValue = 0;
            int index = -1, percende = 100;
            for(int i = 0;i < eq.Length;i++){
                if(eq[i] == '(') {
                    level++;
                }
                else if(eq[i] == ')') level--;

                if(level == 0 && 
                    GetPercende(eq[i]) != 0 && 
                    percende > GetPercende(eq[i])){
                    index = i + correctionValue;
                    percende =  GetPercende(eq[i]);
                }
            }
        
            if(index == -1){
                return new SolveTreeValueNode(eq, parent);
            }
            SolveTreeOperatorNode node = new SolveTreeOperatorNode(eq[index].ToString());
            string left = eq.Substring(0, index);
            if(!char.IsDigit(left[left.Length - 1]) && eq[index] == '-')
                left = "0";
            node.Left = GenerateNode(left,node);
            node.Right = GenerateNode(eq.Substring(index + 1), node);
            node.Parent = parent;
            return node;
        }
    }
}