using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public sealed class ManhattanDistance : IDistance
    {
        public string Name => "Manhattan";

        public double Compute(Grain a, Grain b)
        {
            return
                Math.Abs(a.Area - b.Area) +
                Math.Abs(a.Perimeter - b.Perimeter) +
                Math.Abs(a.Compactness - b.Compactness) +
                Math.Abs(a.KernelLength - b.KernelLength) +
                Math.Abs(a.KernelWidth - b.KernelWidth) +
                Math.Abs(a.AsymmetryCoefficient - b.AsymmetryCoefficient) +
                Math.Abs(a.KernelGrooveLength - b.KernelGrooveLength);
        }
    }
}
