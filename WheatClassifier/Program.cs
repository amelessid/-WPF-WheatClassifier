using System.Diagnostics;
using Spectre.Console;
using WheatClassifier.Classification;
using WheatClassifier.Data;
Console.OutputEncoding = System.Text.Encoding.UTF8;
static void ShowStatus(int k, IDistance distance, int trainSize, int testSize, string reportPath)
{
    var panel = new Panel(
        $"[bold]k[/] : [green]{k}[/]\n" +
        $"[bold]Distance[/] : [green]{distance.Name}[/]\n" +
        $"[bold]Train[/] : {trainSize}  |  [bold]Test[/] : {testSize}\n" +
        $"[bold]Report JSON[/] : {Markup.Escape(reportPath)}"
    )
    .Header("[yellow]Paramètres actuels[/]")
    .Border(BoxBorder.Rounded);

    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();
}

static void ShowConfusion(EvaluationResult eval)
{
    var labels = eval.LabelsOrder;
    var cm = eval.Confusion;

    int n = labels.Length;

    var rowTotals = new int[n];
    var colTotals = new int[n];
    int grandTotal = 0;

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            rowTotals[i] += cm[i, j];
            colTotals[j] += cm[i, j];
            grandTotal += cm[i, j];
        }
    }

    var table = new Table();
    table.Border(TableBorder.Rounded);
    table.Title("[yellow]Matrice de confusion (réel x prédit)[/]");

    table.AddColumn("[bold]Réel \\ Prédit[/]");
    for (int j = 0; j < n; j++)
        table.AddColumn($"[bold]{labels[j]}[/]");
    table.AddColumn("[bold]Total[/]");

    for (int i = 0; i < n; i++)
    {
        var row = new List<string> { $"[bold]{labels[i]}[/]" };

        for (int j = 0; j < n; j++)
        {
            var value = cm[i, j];
            row.Add(i == j ? $"[green]{value}[/]" : value.ToString());
        }

        row.Add($"[bold]{rowTotals[i]}[/]");
        table.AddRow(row.ToArray());
    }

    var totalsRow = new List<string> { "[bold]Total[/]" };
    for (int j = 0; j < n; j++)
        totalsRow.Add($"[bold]{colTotals[j]}[/]");
    totalsRow.Add($"[bold]{grandTotal}[/]");
    table.AddRow(totalsRow.ToArray());

    AnsiConsole.Write(table);
}

static void ShowJsonIfExists(string reportPath)
{
    if (!File.Exists(reportPath))
    {
        AnsiConsole.WriteLine("Aucun report.json trouvé. Lance une classification d’abord.");
        return;
    }

    var json = File.ReadAllText(reportPath);

    AnsiConsole.Write(
        new Panel(json)
            .Header("report.json")
            .Border(BoxBorder.Rounded)
            .Expand()
    );
}

try
{
    AnsiConsole.Write(new FigletText("WheatClassifier").Color(Color.Green));
    AnsiConsole.MarkupLine("[grey]k-NN + KD-Tree | Spectre.Console UI[/]\n");

    var trainPath = Path.Combine(AppContext.BaseDirectory, "DataFiles", "train.csv");
    var testPath = Path.Combine(AppContext.BaseDirectory, "DataFiles", "test.csv");

    var train = CsvLoader.LoadGrains(trainPath, hasHeader: true);
    var test = CsvLoader.LoadGrains(testPath, hasHeader: true);

    int k = 5;
    IDistance distance = new EuclideanDistance();

    var reportPath = Path.Combine(AppContext.BaseDirectory, "report.json");

    while (true)
    {
        ShowStatus(k, distance, train.Count, test.Count, reportPath);

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Menu[/] (↑ ↓ puis Entrée)")
                .AddChoices(new[]
                {
                    "⚙️ Modifier k",
                    "📏 Modifier distance",
                    "▶ Lancer classification",
                    "❌ Quitter"
                }));

        if (choice.StartsWith("⚙️"))
        {
            k = AnsiConsole.Prompt(
                new TextPrompt<int>("Donne la valeur de [yellow]k[/] (1..50) :")
                    .ValidationErrorMessage("[red]k invalide[/]")
                    .Validate(val => val > 0 && val <= 50)
            );

            AnsiConsole.MarkupLine($"✅ k = [green]{k}[/]\n");
        }
        else if (choice.StartsWith("📏"))
        {
            var d = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choisir la [yellow]distance[/] :")
                    .AddChoices("Euclidean", "Manhattan"));

            distance = d == "Euclidean" ? new EuclideanDistance() : new ManhattanDistance();
            AnsiConsole.MarkupLine($"✅ Distance = [green]{distance.Name}[/]\n");
        }
        else if (choice.StartsWith("▶"))
        {
            KDTree? tree = null;
            EvaluationResult? eval = null;

            var swBuild = new Stopwatch();
            var swEval = new Stopwatch();

            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("green"))
                .Start("Exécution...", ctx =>
                {
                    ctx.Status("Construction du KD-Tree...");
                    swBuild.Start();
                    tree = new KDTree(train);
                    swBuild.Stop();

                    ctx.Status("Classification + évaluation...");
                    var knn = new KNNClassifier(tree!, k, distance);

                    swEval.Start();
                    eval = Evaluator.Evaluate(test, knn);
                    swEval.Stop();
                });

            AnsiConsole.MarkupLine($"Accuracy: [bold green]{eval!.Accuracy:P2}[/]");
            AnsiConsole.MarkupLine($"KD-Tree build: [yellow]{swBuild.ElapsedMilliseconds} ms[/]");
            AnsiConsole.MarkupLine($"Eval: [yellow]{swEval.ElapsedMilliseconds} ms[/]\n");

            ShowConfusion(eval);

        }
        else if (choice.StartsWith("❌"))
        {
            break;
        }
    }
}
catch (Exception ex)
{
    AnsiConsole.WriteLine($"Erreur: {ex.Message}");
}
