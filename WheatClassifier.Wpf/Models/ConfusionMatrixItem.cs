namespace WheatClassifier.Wpf.Models
{
    public class ConfusionMatrixItem
    {
        public string ActualClass { get; set; } = "";

        public int Canadian { get; set; }
        public int Kama { get; set; }
        public int Rosa { get; set; }

        public int Total { get; set; }
    }
}