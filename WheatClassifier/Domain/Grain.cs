using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WheatClassifier.Domain
{
    public class Grain : INotifyPropertyChanged
    {
        public double Area { get; set; }
        public double Perimeter { get; set; }
        public double Compactness { get; set; }
        public double KernelLength { get; set; }
        public double KernelWidth { get; set; }
        public double AsymmetryCoefficient { get; set; }
        public double KernelGrooveLength { get; set; }

        public string? Label { get; set; }

        private string? _predictedClass;
        public string? PredictedClass
        {
            get => _predictedClass;
            set
            {
                _predictedClass = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}