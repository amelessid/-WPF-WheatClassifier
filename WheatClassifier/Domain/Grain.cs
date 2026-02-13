using System;
using System.Collections.Generic;
using System.Text;

namespace WheatClassifier.Domain
{
    public class Grain
    {
        public double Area { get; set; }
        public double Perimeter { get; set; }
        public double Compactness { get; set; }
        public double KernelLength { get; set; }
        public double KernelWidth { get; set; }
        public double AsymmetryCoefficient { get; set; }
        public double KernelGrooveLength { get; set; }

        public string? Label { get; set; }
    }
}

