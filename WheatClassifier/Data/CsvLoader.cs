using System;
using System.Collections.Generic;
using System.Text;

using System.Globalization;
using CsvHelper;
using WheatClassifier.Domain;

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using WheatClassifier.Domain;

namespace WheatClassifier.Data
{
    public static class CsvLoader
    {
        public static List<Grain> LoadGrains(string csvPath, bool hasHeader)
        {
            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"Fichier introuvable: {csvPath}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeader,
                Delimiter = ";",
                BadDataFound = null,
                MissingFieldFound = null
            };

            var grains = new List<Grain>();

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);

            if (hasHeader)
                csv.Read(); 

            while (csv.Read())
            {

                double ReadDouble(int index)
                {
                    var s = csv.GetField(index);
                    if (string.IsNullOrWhiteSpace(s))
                        throw new FormatException($"Valeur vide colonne {index} dans {csvPath}");

                    if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                        throw new FormatException($"Valeur numérique invalide '{s}' colonne {index} dans {csvPath}");

                    return v;
                }

                var label = csv.GetField(0);

                var g = new Grain
                {
                    Area = ReadDouble(1),
                    Perimeter = ReadDouble(2),
                    Compactness = ReadDouble(3),
                    KernelLength = ReadDouble(4),
                    KernelWidth = ReadDouble(5),
                    AsymmetryCoefficient = ReadDouble(6),
                    KernelGrooveLength = ReadDouble(7),
                    Label = string.IsNullOrWhiteSpace(label) ? null : label
                };

                grains.Add(g);
            }

            return grains;
        }
    }
}
