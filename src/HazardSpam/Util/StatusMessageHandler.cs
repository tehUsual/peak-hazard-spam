namespace HazardSpam.Util;

public static class StatusMessageHandler
{
    private static PlayerConnectionLog? _playerConnectionLog;


    public static void Init(PlayerConnectionLog playerConnectionLog)
    {
        _playerConnectionLog = playerConnectionLog;
    }

    public static void DeInit()
    {
        _playerConnectionLog = null;
    }

    public static void AddMessage(string message)
    {
        if (_playerConnectionLog != null)
            _playerConnectionLog.AddMessage(message);
    }
}