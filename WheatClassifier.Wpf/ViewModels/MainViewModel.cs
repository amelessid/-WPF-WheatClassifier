using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WheatClassifier.Classification;
using WheatClassifier.Data;
using WheatClassifier.Domain;
using WheatClassifier.Wpf.Commands;
using WheatClassifier.Wpf.Data;
using WheatClassifier.Wpf.Models;
using WheatClassifier.Wpf.Services;
using Microsoft.Win32;
using System.Windows;

namespace WheatClassifier.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Grain> TrainData { get; set; } = new();
        public ObservableCollection<Grain> TestData { get; set; } = new();
        public ObservableCollection<Experiment> Experiments { get; set; } = new();
        public ObservableCollection<ApiProduct> ApiProducts { get; set; } = new();
        public ObservableCollection<ConfusionMatrixItem> ConfusionMatrix { get; set; } = new();


        private int _k = 3;
        public int K
        {
            get => _k;
            set { _k = value; OnPropertyChanged(); }
        }

        private double _accuracy;
        public double Accuracy
        {
            get => _accuracy;
            set { _accuracy = value; OnPropertyChanged(); }
        }

        private string _selectedDistance = "Euclidean";
        public string SelectedDistance
        {
            get => _selectedDistance;
            set { _selectedDistance = value; OnPropertyChanged(); }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand ClassifyCommand { get; }
        public ICommand LoadApiCommand { get; }
        public ICommand LoadTrainCommand { get; }
        public ICommand LoadTestCommand { get; }

        public MainViewModel()
        {
            LoadDataCommand = new RelayCommand(_ => LoadData());
            ClassifyCommand = new RelayCommand(_ => Classify());
            LoadApiCommand = new RelayCommand(async _ => await LoadApi());
            LoadTrainCommand = new RelayCommand(_ => LoadTrain());
            LoadTestCommand = new RelayCommand(_ => LoadTest());

            LoadExperiments();
        }

        private void LoadData()
        {
            string defaultPath = @"C:\Users\lobna\OneDrive\Desktop\TP2\WheatClassifier (2)\WheatClassifier (2)\WheatClassifier\WheatClassifier\DataFiles";

            // 🔹 TRAIN
            var openTrain = new OpenFileDialog
            {
                Title = "Choisir le fichier TRAIN",
                Filter = "CSV files (*.csv)|*.csv",
                InitialDirectory = defaultPath   
            };

            if (openTrain.ShowDialog() == true)
            {
                var train = CsvLoader.LoadGrains(openTrain.FileName, true);

                TrainData.Clear();
                foreach (var item in train)
                    TrainData.Add(item);

                MessageBox.Show($"Train chargé ({TrainData.Count}) ✅");
            }
            else return;

            // 🔹 TEST
            var openTest = new OpenFileDialog
            {
                Title = "Choisir le fichier TEST",
                Filter = "CSV files (*.csv)|*.csv",
                InitialDirectory = defaultPath  
            };

            if (openTest.ShowDialog() == true)
            {
                var test = CsvLoader.LoadGrains(openTest.FileName, true);

                TestData.Clear();
                foreach (var item in test)
                    TestData.Add(item);

                MessageBox.Show($"Test chargé ({TestData.Count}) ✅");
            }
        }
        private void LoadTrain()
        {
            string defaultPath = @"C:\Users\lobna\OneDrive\Desktop\TP2\WheatClassifier (2)\WheatClassifier (2)\WheatClassifier\WheatClassifier\DataFiles";

            var dialog = new OpenFileDialog
            {
                Title = "Choisir TRAIN",
                Filter = "CSV files (*.csv)|*.csv",
                InitialDirectory = defaultPath,   
                FileName = "train.csv"            
            };

            if (dialog.ShowDialog() == true)
            {
                var train = CsvLoader.LoadGrains(dialog.FileName, true);

                TrainData.Clear();
                foreach (var item in train)
                    TrainData.Add(item);

                MessageBox.Show(
    $"Le fichier TRAIN a été chargé avec succès.\n\nNombre de lignes : {TrainData.Count}",
    "Chargement réussi",
    MessageBoxButton.OK,
    MessageBoxImage.Information
);
            }
        }
private void LoadTest()
{
    string defaultPath = @"C:\Users\lobna\OneDrive\Desktop\TP2\WheatClassifier (2)\WheatClassifier (2)\WheatClassifier\WheatClassifier\DataFiles";

    var dialog = new OpenFileDialog
    {
        Title = "Choisir TEST",
        Filter = "CSV files (*.csv)|*.csv",
        InitialDirectory = defaultPath,   
        FileName = "test.csv"             
    };

    if (dialog.ShowDialog() == true)
    {
        var test = CsvLoader.LoadGrains(dialog.FileName, true);

        TestData.Clear();
        foreach (var item in test)
            TestData.Add(item);

                MessageBox.Show(
            $"Le fichier TEST a été chargé avec succès.\n\nNombre de lignes : {TestData.Count}",
            "Chargement réussi",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
            }
}

        private void Classify()
        {
            if (TrainData.Count == 0 || TestData.Count == 0)
            {
                MessageBox.Show(
                    "Veuillez d'abord charger les fichiers TRAIN et TEST.",
                    "Données manquantes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            IDistance distance = SelectedDistance == "Manhattan"
                ? new ManhattanDistance()
                : new EuclideanDistance();

            var trainingList = TrainData.ToList();
            var tree = new KDTree(trainingList);
            var classifier = new KNNClassifier(tree, K, distance);

            int correct = 0;

            foreach (var item in TestData)
            {
                item.PredictedClass = classifier.Predict(item);

                if (item.Label == item.PredictedClass)
                    correct++;
            }

            Accuracy = (double)correct / TestData.Count;

            ConfusionMatrix.Clear();

            var labels = new[] { "Canadian", "Kama", "Rosa" };

            foreach (var actual in labels)
            {
                var rowItems = TestData.Where(g => g.Label == actual).ToList();

                var row = new ConfusionMatrixItem
                {
                    ActualClass = actual,
                    Canadian = rowItems.Count(g => g.PredictedClass == "Canadian"),
                    Kama = rowItems.Count(g => g.PredictedClass == "Kama"),
                    Rosa = rowItems.Count(g => g.PredictedClass == "Rosa")
                };

                ConfusionMatrix.Add(row);
            }

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                var exp = new Experiment
                {
                    K = K,
                    Distance = SelectedDistance,
                    Accuracy = Accuracy
                };

                db.Experiments.Add(exp);
                db.SaveChanges();
            }

            LoadExperiments();

            OnPropertyChanged(nameof(TestData));
        }

        private void LoadExperiments()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                Experiments.Clear();

                foreach (var exp in db.Experiments.OrderByDescending(e => e.Date).ToList())
                {
                    Experiments.Add(exp);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private async Task LoadApi()
        {
            var service = new ApiService();
            var products = await service.GetProducts();

            ApiProducts.Clear();

            foreach (var p in products)
                ApiProducts.Add(p);
        }
    }
}