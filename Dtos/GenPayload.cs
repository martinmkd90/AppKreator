namespace AppKreator.Dtos
{
    public sealed class GenPayload
    {
        public List<ManifestItem> Manifest { get; set; } = new();
        public List<FileItem> Files { get; set; } = new();
        public RunHints? Run { get; set; }
        public string? Notes { get; set; }
    }
    public sealed class ManifestItem
    {
        public string Path { get; set; } = "";
        public string? Language { get; set; }
        public string? Purpose { get; set; }
    }
    public sealed class FileItem
    {
        public string Path { get; set; } = "";
        public string Contents { get; set; } = "";
    }
    public sealed class RunHints
    {
        public string? Instructions { get; set; }
        public string? EntryPoint { get; set; }
        public string? StartCommand { get; set; }
    }

}
