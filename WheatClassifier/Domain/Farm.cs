namespace WheatClassifier.Domain;

public sealed class Farm : IReportable
{
    public string Name { get; }
    public Farmer Owner { get; }                 
    public IReadOnlyList<GrainLot> Lots => _lots; 

    private readonly List<GrainLot> _lots = new();

    public Farm(string name, Farmer owner)
    {
        Name = name;
        Owner = owner;
    }

    public void AddLot(GrainLot lot) => _lots.Add(lot);

    public string GetSummary()
        => $"Farm {Name} | owner={Owner} | lots={_lots.Count}";
}