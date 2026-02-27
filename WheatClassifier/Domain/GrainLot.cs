using WheatClassifier.Classification; 
                                     

namespace WheatClassifier.Domain;

public sealed class GrainLot : IReportable
{
    public string LotId { get; }
    public DateTime CreatedAt { get; }
    public string? Variety { get; }  
    public IReadOnlyList<Grain> Grains => _grains;

    private readonly List<Grain> _grains = new();

    public GrainLot(string lotId, DateTime createdAt, string? variety = null)
    {
        LotId = lotId;
        CreatedAt = createdAt;
        Variety = variety;
    }

    public void AddGrain(Grain g) => _grains.Add(g);

    public string GetSummary()
        => $"Lot {LotId} | grains={_grains.Count} | variety={(Variety ?? "unknown")} | date={CreatedAt:yyyy-MM-dd}";
}