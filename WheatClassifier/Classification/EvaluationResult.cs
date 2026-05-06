using System;
using System.Collections.Generic;
using System.Text;

namespace WheatClassifier.Classification
{
    public sealed class EvaluationResult
    {
        public double Accuracy { get; set; }

        public string[] LabelsOrder { get; set; } = Array.Empty<string>();

        public int[,] Confusion { get; set; } = new int[0, 0];
    }
}
