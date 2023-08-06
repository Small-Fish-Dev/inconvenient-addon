using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Roomm8;

public class Farter
{
    public static Farter The
    {
        get
        {
            _the ??= new Farter();
            return _the;
        }
    }

    private static RangedFloat MicSpamInterval = new (60f, 120f);
    private static RangedFloat GaslightInterval = new (20f, 60f);

    private static Farter _the;

    private bool Active;
    private Sound Current;
    private Queue<MicSpamAsset> Playlist = new();
    private TimeSince LastMicSpam = 0;
    private float NextMicSpam = MicSpamInterval.GetValue();
    private TimeSince LastGaslight = 0;
    private float NextGaslight = GaslightInterval.GetValue();

    private Farter()
    {
        Event.Register(this);
    }

    public void Activate()
    {
        if (!Game.IsServer || Active)
            return;

        Active = true;
    }

    [GameEvent.Tick.Server]
    private void Tick()
    {
        if (!Active)
            return;

        if (!Current.IsPlaying && LastMicSpam > NextMicSpam)
        {
            if (Playlist.Count == 0)
            {
                PopulatePlaylist();
            }

            var s = Playlist.Dequeue();
            var se = s.SoundEvent;
            if (se != default)
            {
                Current = Sound.FromScreen(se);
                LastMicSpam = 0;
                NextMicSpam = MicSpamInterval.GetValue();
            }
        }

        if (LastGaslight > NextGaslight)
        {
            foreach (var victim in Game.Clients)
            {
                if (GaslightMessageAsset.All.Count == 0)
                {
                    break;
                }

                var message = GaslightMessageAsset.All.OrderBy(a => Guid.NewGuid()).First().Message;

                var playersExceptVictim = Game.Clients.Except(new[] { victim }).ToList();
                IClient p = null;
                if (playersExceptVictim.Count > 0)
                {
                    p = playersExceptVictim.OrderBy(a => Guid.NewGuid()).First();
                }

                Chat.AddChatEntry(
                    To.Single(victim),
                    p?.Name ?? "garry",
                    message,
                    p?.SteamId ?? 76561197960279927
                );
            }

            LastGaslight = 0;
            NextGaslight = GaslightInterval.GetValue();
        }
    }

    private void PopulatePlaylist()
    {
        Playlist = new Queue<MicSpamAsset>(MicSpamAsset.All.OrderBy(a => Guid.NewGuid()));
    }
}