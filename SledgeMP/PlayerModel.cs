using SledgeLib;
using System.Numerics;

public class PlayerModel {
    public uint? Body = null;

    public uint? sBody = null;
    public uint? sLeftArm = null;
    public uint? sRightArm = null;
    public uint? sLeftLeg = null;
    public uint? sRightLeg = null;
    public uint? sHead = null;

    private double GetPitch(Quaternion q) {
        return Math.Asin(-2.0*(q.X*q.Z - q.W*q.Y));
    }

    private double GetYaw(Quaternion q) {
        return Math.Atan2(2.0*(q.X*q.Y + q.W*q.Z), q.W*q.W + q.X*q.X - q.Y*q.Y - q.Z*q.Z);
    }

    private double GetRoll(Quaternion q) {
        return Math.Atan2(2.0*(q.X*q.Z + q.W*q.Y), q.W*q.W - q.X*q.X - q.Y*q.Y + q.Z*q.Z);
    }

    public PlayerModel() {
        Body = SledgeLib.Body.Create();
        sBody = Shape.Create(Body.Value);
        sHead = Shape.Create(Body.Value);
        sLeftArm = Shape.Create(Body.Value);
        sRightArm = Shape.Create(Body.Value);
        sLeftLeg = Shape.Create(Body.Value);
        sRightLeg = Shape.Create(Body.Value);

        SledgeLib.Body.SetDynamic(Body.Value, false);
    }

    public void Load() {
        if (Body == null || sBody == null || sHead == null || sLeftArm == null || sRightArm == null || sLeftLeg == null || sRightLeg == null)
            return;

        Shape.LoadVox(sBody.Value, "Assets/Models/Player.vox", "body", 1f);
        Shape.LoadVox(sHead.Value, "Assets/Models/Player.vox", "head", 1f);
        Shape.LoadVox(sLeftArm.Value, "Assets/Models/Player.vox", "arm_left", 1f);
        Shape.LoadVox(sRightArm.Value, "Assets/Models/Player.vox", "arm_right", 1f);
        Shape.LoadVox(sLeftLeg.Value, "Assets/Models/Player.vox", "leg_left", 1f);
        Shape.LoadVox(sRightLeg.Value, "Assets/Models/Player.vox", "leg_right", 1f);

        Log.General("Loaded all player voxes");

        SledgeLib.Body.SetTransform((uint)Body, new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0, 0, 1)));
    }

    public void Update(Vector3 startPos, Vector3 endPos, float t, Quaternion rot) {
        SledgeLib.Body.SetPosition(Body!.Value, Vector3.Lerp(startPos, endPos, 1));

        Shape.SetLocalTransform(sBody!.Value, new Transform(new Vector3(0.3f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));
        Shape.SetLocalTransform(sHead!.Value, new Transform(new Vector3(0.3f, 1.4f, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));
        Shape.SetLocalTransform(sLeftArm!.Value, new Transform(new Vector3(-0.2f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));
        Shape.SetLocalTransform(sRightArm!.Value, new Transform(new Vector3(0.4f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));
        Shape.SetLocalTransform(sLeftLeg!.Value, new Transform(new Vector3(0, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));
        Shape.SetLocalTransform(sRightLeg!.Value, new Transform(new Vector3(0.3f, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));

        Quaternion camRot = Player.GetCameraTransform().Rotation;
        SledgeLib.Body.SetRotation(Body!.Value, Quaternion.CreateFromYawPitchRoll((float)GetPitch(camRot), 0, 0));
        SledgeLib.Body.SetDynamic(Body!.Value, false);
    }

    public void Remove() {
        if (Body != null)
            SledgeLib.Body.Destroy(Body.Value);
    }
}