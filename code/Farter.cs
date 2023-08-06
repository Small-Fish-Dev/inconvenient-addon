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

    private static RangedFloat _micSpamInterval = new(60f, 120f);
    private static RangedFloat _gaslightInterval = new(20f, 60f);
    private static RangedFloat _inconvenientRockInterval = new(20, 60);

    private static Farter _the;

    private bool Active;
    private Sound Current;
    private Queue<MicSpamAsset> Playlist = new();
    private TimeSince LastMicSpam = 0;
    private float NextMicSpam = _micSpamInterval.GetValue();
    private TimeSince LastGaslight = 0;
    private float NextGaslight = _gaslightInterval.GetValue();
    private TimeSince LastInconvenientRock = 0;
    private float NextInconvenientRock = _inconvenientRockInterval.GetValue();

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
                NextMicSpam = _micSpamInterval.GetValue();
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
                if (playersExceptVictim.Count > 0 &&
                    new RangedFloat(0, 100).GetValue() > 20) // 20% chance of getting message from garry 
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
            NextGaslight = _gaslightInterval.GetValue();
        }

        if (LastInconvenientRock > NextInconvenientRock)
        {
            var iterations = 3;
            var bestPosition = Vector3.Zero;
            var bounds = Game.PhysicsWorld.Body.GetBounds();
            var worldHeight = (bounds.Maxs - bounds.Mins).z;

            while (iterations > 0)
            {
                var randomPosition = bounds.RandomPointInside;
                var traceDown = Trace.Ray(randomPosition.WithZ(bounds.Maxs.z),
                        randomPosition.WithZ(bounds.Maxs.z) + Vector3.Down * worldHeight).StaticOnly()
                    .Run();
                bestPosition = traceDown.Hit ? traceDown.EndPosition : randomPosition;

                var atLeastOneSeesTheRock = false;
                foreach (var cl in Game.Clients)
                {
                    var pawn = cl.Pawn;
                    if (pawn is not AnimatedEntity player)
                        continue;

                    if (Math.Abs(
                            Rotation.LookAt(bestPosition - pawn.Position).Yaw() - pawn.Rotation.Yaw()
                        ) < 45)
                    {
                        // The rock is in player's FOV, let's see if they really see it
                        var t = Trace.Ray(bestPosition, player.Position).WithTag("player").Run();
                        if (t.Hit && t.Entity == player)
                        {
                            //Log.Info($"Player {cl.Name} sees the rock at {bestPosition}");
                            atLeastOneSeesTheRock = true;
                            break;
                        }
                    }
                }

                if (atLeastOneSeesTheRock)
                {
                    //Log.Info("At least one sees the rock");
                    iterations--;
                }
                else
                    break;
            }

            _ = new InconvenientRock { Position = bestPosition };
#if DEBUG
            //Log.Info($"spawned the rock at {bestPosition} after {3 - iterations} iterations");
#endif
            LastInconvenientRock = 0;
            NextInconvenientRock = _inconvenientRockInterval.GetValue();
        }
    }

    private void PopulatePlaylist()
    {
        Playlist = new Queue<MicSpamAsset>(MicSpamAsset.All.OrderBy(a => Guid.NewGuid()));
    }
}