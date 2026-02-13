using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public static class Evaluator
    {
        public static EvaluationResult Evaluate(List<Grain> testSet, IClassifier classifier)
        {
            if (testSet == null || testSet.Count == 0)
                throw new ArgumentException("Test set vide.");
            if (classifier == null)
                throw new ArgumentNullException(nameof(classifier));

            // On définit l’ordre des classes (important pour une matrice stable)
            var labels = testSet
                .Select(g => g.Label)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(l => l) // ordre alphabétique stable
                .ToArray()!;

            // On force 3 classes si tu veux fixer l’ordre
            // (décommente si ton prof exige Kama/Rosa/Canadian dans un ordre précis)
            // var labels = new[] { "Kama", "Rosa", "Canadian" };

            var index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < labels.Length; i++)
                index[labels[i]] = i;

            var cm = new int[labels.Length, labels.Length];

            int correct = 0;

            foreach (var g in testSet)
            {
                if (string.IsNullOrWhiteSpace(g.Label))
                    continue; // impossible d’évaluer sans vrai label

                var predicted = classifier.Predict(g);
                var actual = g.Label;

                if (!index.ContainsKey(actual))
                    continue;

                // si le classifieur renvoie un label inattendu, on peut l’ignorer ou le traiter
                if (!index.ContainsKey(predicted))
                {
                    // Ici: on ignore la prédiction hors labels
                    continue;
                }

                int i = index[actual];
                int j = index[predicted];
                cm[i, j]++;

                if (predicted.Equals(actual, StringComparison.OrdinalIgnoreCase))
                    correct++;
            }

            double accuracy = (double)correct / testSet.Count;

            return new EvaluationResult
            {
                Accuracy = accuracy,
                LabelsOrder = labels,
                Confusion = cm
            };
        }
    }
}
