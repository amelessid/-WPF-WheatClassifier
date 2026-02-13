using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public sealed class EuclideanDistance : IDistance
    {
        public string Name => "Euclidean";

        public double Compute(Grain a, Grain b)
        {
            double sum =
                Sq(a.Area - b.Area) +
                Sq(a.Perimeter - b.Perimeter) +
                Sq(a.Compactness - b.Compactness) +
                Sq(a.KernelLength - b.KernelLength) +
                Sq(a.KernelWidth - b.KernelWidth) +
                Sq(a.AsymmetryCoefficient - b.AsymmetryCoefficient) +
                Sq(a.KernelGrooveLength - b.KernelGrooveLength);

            return Math.Sqrt(sum);
        }

        private static double Sq(double x) => x * x;
    }
}
