namespace Project11E;

public class ProjectConstants
{
    public const string DatabaseProvider = "SqlServer";

    public const string SqlServerConnectionString = @"Data Source=.\SECONDSON; " +
                "Initial Catalog=Northwind; " +
                "Integrated Security = true; " +
                "Trust Server Certificate = true; " +
                "MultipleActiveResultSets=true; ";
}
