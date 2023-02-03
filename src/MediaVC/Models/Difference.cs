using MediaVC.Enums;

namespace MediaVC.Models
{
    public class Difference
    {
        public string? OldPath { get; set; }

        public string Path { get; set; } = string.Empty;

        public ChangeMode ChangeMode { get; set; } = ChangeMode.Create;

        public DifferenceInfo? Info { get; set; }
    }
}
