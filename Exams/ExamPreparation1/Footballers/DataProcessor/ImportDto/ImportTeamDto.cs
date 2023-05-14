namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamDto
    {
        public string? Name { get; set; }
        public string? Nationality { get; set; }
        public string? Trophies { get; set; }
        public HashSet<int>? Footballers { get; set; }
    }
}
