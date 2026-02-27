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

            var labels = testSet
                .Select(g => g.Label)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(l => l) 
                .ToArray()!;



            var index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < labels.Length; i++)
                index[labels[i]] = i;

            var cm = new int[labels.Length, labels.Length];

            int correct = 0;

            foreach (var g in testSet)
            {
                if (string.IsNullOrWhiteSpace(g.Label))
                    continue;

                var predicted = classifier.Predict(g);
                var actual = g.Label;

                if (!index.ContainsKey(actual))
                    continue;


                if (!index.ContainsKey(predicted))
                {
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
