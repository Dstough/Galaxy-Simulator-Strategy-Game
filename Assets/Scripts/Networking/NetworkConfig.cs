public static class NetworkConfig
{
    public static string IpAddress { get; set; }
    public static string UserName { get; set; }
    public static bool IsServer { get; set; }

    public const int PORT = 18887;
    public const int BUFFER_SIZE = 4096;
}