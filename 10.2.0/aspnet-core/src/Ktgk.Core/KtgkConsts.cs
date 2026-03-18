using Ktgk.Debugging;

namespace Ktgk;

public class KtgkConsts
{
    public const string LocalizationSourceName = "Ktgk";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "e75a097dcc7548cd87a6b0bb8f4ac324";
}
