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

//#if DEBUG
    //private const float MicSpamInterval = 10f;
    //private const float GaslightInterval = 10f;
//#else
    private const float MicSpamInterval = 60f;
    private const float GaslightInterval = 40f;
//#endif

    private static Farter _the;

    private bool Active;
    private Sound Current;
    private Queue<MicSpamAsset> Playlist = new();
    private TimeSince LastMicSpam = 0;
    private TimeSince LastGaslight = 0;

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

        if (!Current.IsPlaying && LastMicSpam > MicSpamInterval)
        {
            if (Playlist.Count == 0)
            {
#if DEBUG
                //Log.Info("Reached the end of playlist, making a new one...");
#endif
                PopulatePlaylist();
            }

            var s = Playlist.Dequeue();
            var se = s.SoundEvent;
            if (se == default)
            {
                // Try the next sound if this one fails
#if DEBUG
                //Log.Error($"Failed to play {s.ResourcePath}: empty SoundEvent");
#endif
                return;
            }

            Current = Sound.FromScreen(se);
            LastMicSpam = 0;
        }

        if (LastGaslight > GaslightInterval)
        {
            foreach (var victim in Game.Clients)
            {
                if (GaslightMessageAsset.All.Count == 0)
                {
#if DEBUG
                    //Log.Error("No gaslight messages found");
#endif
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
        }
    }

    private void PopulatePlaylist()
    {
        Playlist = new Queue<MicSpamAsset>(MicSpamAsset.All.OrderBy(a => Guid.NewGuid()));
    }
}