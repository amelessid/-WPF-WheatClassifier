using WheatClassifier.Domain;

namespace WheatClassifier.Classification
{
    public interface IClassifier
    {
        string Predict(Grain x);
    }
}
