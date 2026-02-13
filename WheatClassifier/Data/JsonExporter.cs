using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using WheatClassifier.Classification;

namespace WheatClassifier.Data
{
    public static class JsonExporter
    {
        public static void SaveReport(
            string outputPath,
            int k,
            IDistance distance,
            int trainSize,
            int testSize,
            EvaluationResult eval)
        {
            // Convertir int[,] -> int[][]
            int n = eval.Confusion.GetLength(0);
            var jagged = new int[n][];
            for (int i = 0; i < n; i++)
            {
                jagged[i] = new int[n];
                for (int j = 0; j < n; j++)
                    jagged[i][j] = eval.Confusion[i, j];
            }

            var report = new GlobalReport
            {
                DateUtc = DateTime.UtcNow.ToString("o"),
                K = k,
                Distance = distance.Name,
                TrainSize = trainSize,
                TestSize = testSize,
                Accuracy = eval.Accuracy,
                LabelsOrder = eval.LabelsOrder,
                ConfusionMatrix = jagged
            };

            var json = JsonConvert.SerializeObject(report, Formatting.Indented);
            File.WriteAllText(outputPath, json);
        }
    }
}
