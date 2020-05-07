namespace NodeManager.Models
{
    public class NodeJsRow
    {
        public string NodeVersion { get; set; }
        public string NpmVersion { get; set; }
        public string ReleaseDate { get; set; }
        public bool IsLts { get; set; }
        public string LtsName { get; set; }
        public string DistUrl { get; set; }
        public bool IsInstalled { get; set; }
        public string InstallationSize { get; set; }
        public string InstallationPath { get; set; }
    }
}