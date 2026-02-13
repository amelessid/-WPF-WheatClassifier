using System;
using System.Collections.Generic;
using System.Text;

namespace WheatClassifier.Classification
{
    public sealed class EvaluationResult
    {
        public double Accuracy { get; set; }

        // Ordre des classes utilisé pour la matrice
        public string[] LabelsOrder { get; set; } = Array.Empty<string>();

        // Confusion[i,j] : réel = i, prédit = j
        public int[,] Confusion { get; set; } = new int[0, 0];
    }
}
