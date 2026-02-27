using System;
using System.Collections.Generic;
using System.Text;

namespace WheatClassifier.Data
{
    public sealed class GlobalReport
    {
        public string DateUtc { get; set; } = "";
        public int K { get; set; }
        public string Distance { get; set; } = "";

        public int TrainSize { get; set; }
        public int TestSize { get; set; }

        public double Accuracy { get; set; }


        public int[][] ConfusionMatrix { get; set; } = Array.Empty<int[]>();

        public string[] LabelsOrder { get; set; } = Array.Empty<string>();
    }
}

