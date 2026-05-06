using System;

namespace WheatClassifier.Wpf.Models
{
    public class Experiment
    {
        public int Id { get; set; }
        public int K { get; set; }
        public string Distance { get; set; } = "";
        public double Accuracy { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}