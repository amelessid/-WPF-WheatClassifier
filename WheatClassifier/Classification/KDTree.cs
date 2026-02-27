using WheatClassifier.Domain;
using WheatClassifier.Utils;

namespace WheatClassifier.Classification
{
    public sealed class KDTree
    {
        public KDNode? Root { get; }

        public KDTree(List<Grain> points)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException("Liste d'apprentissage vide.");

            Root = Build(points, depth: 0);
        }

        private static KDNode? Build(List<Grain> points, int depth)
        {
            if (points.Count == 0) return null;

            int axis = depth % GrainFeatures.Dimensions;

            var sorted = Sorter.MergeSortByKey(points, p => GrainFeatures.Get(p, axis));

            int medianIndex = sorted.Count / 2;
            var node = new KDNode(sorted[medianIndex], axis);

            var leftPoints = sorted.GetRange(0, medianIndex);
            var rightPoints = sorted.GetRange(medianIndex + 1, sorted.Count - (medianIndex + 1));

            node.Left = Build(leftPoints, depth + 1);
            node.Right = Build(rightPoints, depth + 1);

            return node;
        }
        private sealed class Neighbor
        {
            public Grain Point { get; }
            public double Dist { get; }

            public Neighbor(Grain point, double dist)
            {
                Point = point;
                Dist = dist;
            }
        }

        public List<Grain> SearchKNearest(Grain target, int k, IDistance distance)
        {
            if (k <= 0) throw new ArgumentOutOfRangeException(nameof(k));
            var best = new List<Neighbor>(k);

            SearchRecursive(Root, target, k, distance, best);

            var result = new List<Grain>(best.Count);
            for (int i = 0; i < best.Count; i++)
                result.Add(best[i].Point);

            return result;
        }

        private static void SearchRecursive(
            KDNode? node,
            Grain target,
            int k,
            IDistance distance,
            List<Neighbor> best)
        {
            if (node == null) return;

            double d = distance.Compute(target, node.Point);
            InsertBest(best, new Neighbor(node.Point, d), k);

            int axis = node.Axis;
            double diff = GrainFeatures.Get(target, axis) - GrainFeatures.Get(node.Point, axis);

            KDNode? near = diff <= 0 ? node.Left : node.Right;
            KDNode? far = diff <= 0 ? node.Right : node.Left;

            SearchRecursive(near, target, k, distance, best);

            double worstDist = best.Count < k ? double.PositiveInfinity : best[best.Count - 1].Dist;
            if (Math.Abs(diff) < worstDist)
            {
                SearchRecursive(far, target, k, distance, best);
            }
        }

        private static void InsertBest(List<Neighbor> best, Neighbor candidate, int k)
        {
            int pos = 0;
            while (pos < best.Count && best[pos].Dist <= candidate.Dist)
                pos++;

            best.Insert(pos, candidate);

            if (best.Count > k)
                best.RemoveAt(best.Count - 1);
        }


    }
}
