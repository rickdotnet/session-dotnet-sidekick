namespace Dusty.Shared;

public record AppConfig
{
    public string GroqApiKey { get; set; } = Secrets.GroqApiKey;
    public string GitHubApiKey { get; set; } = Secrets.GitHubApiKey;
    public string NatsUrl { get; set; } = "nats://localhost:4222";
    public string NatsUser { get; set; } = "rick";
    public string NatsPassword { get; set; } = "changeme"; // not used in demo
    public string DustyWorkingDirectory 
        => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.dusty";
    public string ObsidianVaultPath 
        => Path.Combine(DustyWorkingDirectory, "ObsidianVault");
}
