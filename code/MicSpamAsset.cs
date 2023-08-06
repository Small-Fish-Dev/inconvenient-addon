using System.Collections.Generic;
using Sandbox;

namespace Roomm8;

[GameResource("Mic Spam Sound", "micspam", "Some annoying sound", Icon = "explosion")]
public class MicSpamAsset : GameResource
{
    [ResourceType("sound")]
    public string SoundEvent { get; set; }

    public static IReadOnlyList<MicSpamAsset> All => _all;
    internal static List<MicSpamAsset> _all = new();

    protected override void PostLoad()
    {
        base.PostLoad();

        if (!_all.Contains(this))
            _all.Add(this);
    }
}