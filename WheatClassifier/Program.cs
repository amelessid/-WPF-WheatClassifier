using Spectre.Console;
using WheatClassifier.Classification;
using WheatClassifier.Data;

static void ShowConfusion(EvaluationResult eval)
{
    var labels = eval.LabelsOrder;
    var cm = eval.Confusion;

    var table = new Table();
    table.Border(TableBorder.Rounded);
    table.Title("[yellow]Matrice de confusion (réel x prédit)[/]");

    table.AddColumn("[bold]Réel \\ Prédit[/]");
    for (int j = 0; j < labels.Length; j++)
        table.AddColumn($"[bold]{labels[j]}[/]");

    for (int i = 0; i < labels.Length; i++)
    {
        var row = new List<string> { $"[bold]{labels[i]}[/]" };
        for (int j = 0; j < labels.Length; j++)
            row.Add(cm[i, j].ToString());

        table.AddRow(row.ToArray());
    }

    AnsiConsole.Write(table);
}

try
{
    AnsiConsole.MarkupLine("[green]=== WheatClassifier (k-NN + KD-Tree) ===[/]");

    // Charger données une seule fois
    var trainPath = Path.Combine(AppContext.BaseDirectory, "DataFiles", "train.csv");
    var testPath = Path.Combine(AppContext.BaseDirectory, "DataFiles", "test.csv");

    var train = CsvLoader.LoadGrains(trainPath, hasHeader: true);
    var test = CsvLoader.LoadGrains(testPath, hasHeader: true);

    int k = 5;
    IDistance distance = new EuclideanDistance();

    while (true)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Menu[/]")
                .AddChoices(new[]
                {
                    "1) Choisir k",
                    "2) Choisir distance",
                    "3) Lancer classification",
                    "4) Quitter"
                }));

        if (choice.StartsWith("1"))
        {
            k = AnsiConsole.Prompt(
                new TextPrompt<int>("Donne la valeur de [yellow]k[/] (ex: 1..25) :")
                    .ValidationErrorMessage("[red]k invalide[/]")
                    .Validate(val => val > 0 && val <= 50)
            );

            AnsiConsole.MarkupLine($"k = [green]{k}[/]");
        }
        else if (choice.StartsWith("2"))
        {
            var d = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choisir la [yellow]distance[/] :")
                    .AddChoices("Euclidean", "Manhattan"));

            distance = d == "Euclidean" ? new EuclideanDistance() : new ManhattanDistance();
            AnsiConsole.MarkupLine($"Distance = [green]{distance.Name}[/]");
        }
        else if (choice.StartsWith("3"))
        {
            AnsiConsole.MarkupLine("[cyan]Construction du KD-Tree...[/]");
            var tree = new KDTree(train);

            AnsiConsole.MarkupLine("[cyan]Classification...[/]");
            var knn = new KNNClassifier(tree, k, distance);

            var eval = Evaluator.Evaluate(test, knn);

            AnsiConsole.MarkupLine($"Accuracy: [bold green]{eval.Accuracy:P2}[/]");
            ShowConfusion(eval);

            var outputPath = Path.Combine(AppContext.BaseDirectory, "report.json");
            JsonExporter.SaveReport(outputPath, k, distance, train.Count, test.Count, eval);

            AnsiConsole.MarkupLine($"[green]JSON sauvegardé:[/] {outputPath}");
        }
        else if (choice.StartsWith("4"))
        {
            break;
        }

        AnsiConsole.WriteLine();
    }
}

catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Erreur:[/] {ex.Message}");
}

