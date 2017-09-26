namespace GraphicsResearch.PathPlacement
{
    using System.Collections.Generic;

    public class DisjointSets<T>
    {
        private Dictionary<T, DisjointSetData> set;

        public DisjointSets(List<T> setItems)
        {
            this.set = new Dictionary<T, DisjointSetData>();
            foreach (T item in setItems)
                this.set.Add(item, new DisjointSetData(item, 0));
        }
        
        public bool IsSameSet(T u, T v)
        {
            return GetRoot(u).Equals(GetRoot(v));
        }

        public void Merge(T u, T v)
        {
            if (IsSameSet(u, v))
                return;

            if (set[u].Rank < set[v].Rank)
            {
                T root = GetRoot(u);
                set[GetRoot(v)].Reference = root;
                set[root].Rank++;
            }
            else
            {
                T root = GetRoot(v);
                set[GetRoot(u)].Reference = root;
                set[root].Rank++;
            }
        }

        private T GetRoot(T item)
        {
            if (set[item].Reference.Equals(item))
                return item;

            set[item].Reference = GetRoot(set[item].Reference);
            return set[item].Reference;
        }

        private class DisjointSetData
        {
            public T Reference { get; set; }

            public int Rank { get; set; }

            public DisjointSetData(T reference, int rank)
            {
                this.Reference = reference;
                this.Rank = rank;
            }
        }
    }
}
