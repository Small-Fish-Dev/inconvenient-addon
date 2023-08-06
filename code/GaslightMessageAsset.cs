using System.Collections.Generic;
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
}