using Sandbox;

namespace Roomm8;

public class InconvenientRock : ModelEntity
{
    private static RangedFloat _randomYaw = new(0, 360);

    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/rust_nature/rocks/rock_cliff_a.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Static);

        Rotation = Rotation.FromYaw(_randomYaw.GetValue());
    }
    
    protected override void OnDestroy()
    {
        if (Game.IsServer)
        {
            //Log.Info("haha fuck you");
            _ = new InconvenientRock { Position = Position, Rotation = Rotation };
        }

        base.OnDestroy();
    }
}