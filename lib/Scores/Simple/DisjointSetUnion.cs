using System.Linq;

namespace lib.Scores.Simple
{
    public class DisjointSetUnion
    {
        public int[] root;
        public int[] treeSize;

        public DisjointSetUnion(int size)
        {
            root = Enumerable.Repeat(-1, size).ToArray();
            treeSize = Enumerable.Repeat(1, size).ToArray();
        }

        public bool SameSet(int x, int y)
        {
            var xRoot = Root(x);
            var yRoot = Root(y);
            return xRoot == yRoot;
        }

        public bool Add(int x, int y)
        {
            var xRoot = Root(x);
            var yRoot = Root(y);
            if (xRoot == yRoot)
                return false;
            if (treeSize[xRoot] < treeSize[yRoot])
            {
                var t = yRoot;
                yRoot = xRoot;
                xRoot = t;
            }
            treeSize[xRoot] += treeSize[yRoot];
            root[y] = x;
            return true;
        }

        private int Root(int x)
        {
            return root[x] < 0 ? x : (root[x] = Root(root[x]));
        }
    }
}