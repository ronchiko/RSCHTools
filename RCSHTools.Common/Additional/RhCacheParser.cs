using System;
using System.IO;

namespace RCSHTools
{
    public static class RhCacheParser {

        public static BinaryTree<string> ParseCache(string path){
            string[] files = File.ReadAllLines(path);
            BinaryTree<string> cache = new BinaryTree<string>((string x, string y) => {
                int len = Math.Min(x.Length, y.Length);
                for(int i = 0; i < len; i++)
                {
                    if (x[i] > y[i]) return 1;
                    else if (x[i] < y[i]) return -1;
                }

                if (x.Length > y.Length) return -1;
                else if (x.Length < y.Length) return 1;

                return 0;
            });

            foreach (string file in files)
            {
                cache.Add(file);
            }

            return cache;
        }

        public static BinaryTree<string, string> ParseFromCache(string path){
            string[] lines = File.ReadAllLines(path);

            return BinaryTree<string,string>.LoadFromCache(lines);
        }

        public static void CreateTreeCache<T1,T2>(string path, BinaryTree<T1,T2> tree, bool canCreate = true){
            tree.CreateCacheFile(path, false);
        }
    }
}