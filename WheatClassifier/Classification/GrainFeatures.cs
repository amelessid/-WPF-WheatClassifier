using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public static class GrainFeatures
    {
        // 7 dimensions (0..6)
        public const int Dimensions = 7;

        public static double Get(Grain g, int axis) => axis switch
        {
            0 => g.Area,
            1 => g.Perimeter,
            2 => g.Compactness,
            3 => g.KernelLength,
            4 => g.KernelWidth,
            5 => g.AsymmetryCoefficient,
            6 => g.KernelGrooveLength,
            _ => throw new ArgumentOutOfRangeException(nameof(axis))
        };
    }
}
