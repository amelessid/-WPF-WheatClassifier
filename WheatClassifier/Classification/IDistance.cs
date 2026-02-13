using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public interface IDistance
    {
        double Compute(Grain a, Grain b);
        string Name { get; }
    }
}
