namespace DockerShadow.Domain.Settings;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
}
