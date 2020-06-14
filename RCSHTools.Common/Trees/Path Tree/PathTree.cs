using System.Collections.Generic;

namespace RCSHTools {

    internal class PathNode<T>{
        public List<PathNode<T>> children;
        public string ID {get; set;}
        public T Value {get; set;}
        public bool IsLeaf => children.Count == 0;

        public PathNode(string id, T v){
            children = new List<PathNode<T>>();
            ID = id;
            Value = v;
        }

        internal PathNode<T> GetChild(string id){
            for (int i = 0; i < children.Count; i++)
            {
                if(children[i].ID == id) return children[i];
            }
            return null;
        }

        internal void AddChild(string id, T value){
            children.Add(new PathNode<T>(id, value));
        }
        internal void AddChild(PathNode<T> node){
            children.Add(node);
        }
    }

    public class PathTree<T> {
        PathNode<T> origin;
        bool rootOrigin;

        public PathTree(){
            origin = new PathNode<T>("__root__", default(T));
            rootOrigin = true;
        }

        public PathTree(string origin, T value){
            this.origin = new PathNode<T>(origin, value);
            rootOrigin = false;
        }
    
        public void Add(string path, T value){
            if(rootOrigin){
                AddNode("__root__/" + path, value);
            }else{
                AddNode(path, value);
            }
        }

        private void AddNode(string path, T value){
            int s = 0;
            int e = path.IndexOf('/');
            PathNode<T> n = origin;
            while(e != -1){
                string name = path.Substring(s, e - s); 
                PathNode<T> node = n.GetChild(name);
                if(node == null){
                    node = new PathNode<T>(name, default(T));
                    n.AddChild(node);
                }
                n = node;
                s = e + 1;
                e = path.IndexOf('/',s);
            }

            n.Value = value;
        }
    
        public T Get(string path){
            if(rootOrigin){
                path = "__root__/" + path;
            }
            return GetNode(path).Value;
        }

        public void Set(string path, T value){
            if(rootOrigin){
                path = "__root__/" + path;
            }
            GetNode(path).Value = value;
        }

        private PathNode<T> GetNode(string path){
            int s = 0;
            int e = path.IndexOf('/');
            PathNode<T> n = origin;
            while(e != -1){
                string name = path.Substring(s, e - s); 
                PathNode<T> node = n.GetChild(name);
                if(node == null){
                    throw new System.Exception("No path continuation");
                }
                n = node;
                s = e + 1;
                e = path.IndexOf('/',s);
            }

            return n;
        }
    }
}