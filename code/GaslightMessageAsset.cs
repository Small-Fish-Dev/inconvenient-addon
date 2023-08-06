using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Roomm8;

[GameResource("Gaslight Message", "gaslight", "Some weird message", Icon = "explosion")]
public class GaslightMessageAsset : GameResource
{
    public string Message { get; set; }

    public static IReadOnlyList<GaslightMessageAsset> All => _all;
    internal static List<GaslightMessageAsset> _all = new();

    protected override void PostLoad()
    {
        base.PostLoad();

        if (!_all.Contains(this))
            _all.Add(this);
    }

/*#if DEBUG
    [ConCmd.Admin("gaslight_all")]
    private static void PrintAllGaslightMessages()
    {
        foreach (var message in All.OrderBy(x => x.Message))
        {
            Log.Info($"{message.Message}");
        }
    }
#endif*/
}