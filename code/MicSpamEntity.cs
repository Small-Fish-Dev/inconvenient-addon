using Sandbox;

namespace Roomm8;

public class MicSpamEntity : Prop
{
    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/hampter/hampter.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        
        Farter.The.Activate();
    }
}