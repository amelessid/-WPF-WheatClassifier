using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public sealed class KNNClassifier : IClassifier
    {
        private readonly KDTree _tree;
        private readonly int _k;
        private readonly IDistance _distance;

        public KNNClassifier(KDTree tree, int k, IDistance distance)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
            _distance = distance ?? throw new ArgumentNullException(nameof(distance));

            if (k <= 0) throw new ArgumentOutOfRangeException(nameof(k));
            _k = k;
        }

        public string Predict(Grain x)
        {
            var neighbors = _tree.SearchKNearest(x, _k, _distance);

            // Vote majoritaire
            // (simple et clair : compter les labels)
            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var n in neighbors)
            {
                if (string.IsNullOrWhiteSpace(n.Label))
                    continue;

                if (!counts.ContainsKey(n.Label))
                    counts[n.Label] = 0;

                counts[n.Label]++;
            }

            if (counts.Count == 0)
                return "Unknown";

            // Trouver la classe la plus fréquente
            // En cas d'égalité, on choisit la classe du voisin le plus proche parmi les tied
            int bestCount = counts.Values.Max();
            var tiedLabels = counts.Where(kv => kv.Value == bestCount).Select(kv => kv.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (tiedLabels.Count == 1)
                return tiedLabels.First();

            // Tie-break: premier voisin (le plus proche) dont le label est dans tiedLabels
            foreach (var n in neighbors)
            {
                if (!string.IsNullOrWhiteSpace(n.Label) && tiedLabels.Contains(n.Label))
                    return n.Label;
            }

            return tiedLabels.First();
        }
    }
}
