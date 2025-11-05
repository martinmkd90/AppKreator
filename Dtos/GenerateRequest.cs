namespace AppKreator.Dtos
{
    public sealed class GenerateRequest
    {
        public string Idea { get; set; } = "";
        public string? UiFramework { get; set; } = "Angular";
        public string? Language { get; set; } = "TypeScript";
        public string? Backend { get; set; } = "ASP.NET";
        public string? Constraints { get; set; } = "";
        public string? DefaultStack { get; set; } = "Angular+TS+ASP.NET";
        public string? Model { get; set; } // allow override
    }

}
