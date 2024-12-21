namespace AngularWithASP.Server.Models;

public class TransactionsChartData
{
    public string? date { get; set; }
    public long amount { get; set; }
}

public class TransactionsChart
{
    public string? Type { get; set; }
    public List<TransactionsChartData>? Data { get; set; }
}
