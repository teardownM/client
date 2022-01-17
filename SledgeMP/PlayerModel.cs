using SledgeLib;
using System.Numerics;

public class PlayerModel {
    public CBody? Body = null;

    public CShape? sBody = null;
    public CShape? sLeftArm = null;
    public CShape? sRightArm = null;
    public CShape? sLeftLeg = null;
    public CShape? sRightLeg = null;
    public CShape? sHead = null;

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
        Body = new CBody();
        sBody = new CShape(Body);
        sHead = new CShape(Body);
        sLeftArm = new CShape(Body);
        sRightArm = new CShape(Body);
        sLeftLeg = new CShape(Body);
        sRightLeg = new CShape(Body);

        Tags.SetTag(Body.m_Handle, "unbreakable", "");
    }

    public void Load() {
        if (Body == null || sBody == null || sHead == null || sLeftArm == null || sRightArm == null || sLeftLeg == null || sRightLeg == null)
            return;

        CShape.LoadVox(sBody.m_Handle, "Assets/Models/Player.vox", "body", 1f);
        CShape.LoadVox(sHead.m_Handle, "Assets/Models/Player.vox", "head", 1f);
        CShape.LoadVox(sLeftArm.m_Handle, "Assets/Models/Player.vox", "arm_left", 1f);
        CShape.LoadVox(sRightArm.m_Handle, "Assets/Models/Player.vox", "arm_right", 1f);
        CShape.LoadVox(sLeftLeg.m_Handle, "Assets/Models/Player.vox", "leg_left", 1f);
        CShape.LoadVox(sRightLeg.m_Handle, "Assets/Models/Player.vox", "leg_right", 1f);

        Log.General("Loaded all player voxes");

        sBody.m_LocalTransform = new Transform(new Vector3(0.3f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sHead.m_LocalTransform = new Transform(new Vector3(0.3f, 1.4f, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sLeftArm.m_LocalTransform = new Transform(new Vector3(-0.2f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sRightArm.m_LocalTransform = new Transform(new Vector3(0.4f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sLeftLeg.m_LocalTransform = new Transform(new Vector3(0, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sRightLeg.m_LocalTransform = new Transform(new Vector3(0.3f, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));

        Tags.SetTag(sBody.m_Handle, "nocull", "");
        Tags.SetTag(sHead.m_Handle, "nocull", "");
        Tags.SetTag(sLeftArm.m_Handle, "nocull", "");
        Tags.SetTag(sRightArm.m_Handle, "nocull", "");
        Tags.SetTag(sLeftLeg.m_Handle, "nocull", "");
        Tags.SetTag(sRightLeg.m_Handle, "nocull", "");

        Body.m_Dynamic = true;
    }

    public void Update(Vector3 startPos, Vector3 endPos, Quaternion playerRotation) {
        Body!.m_Velocity = new Vector3(0, 0, 0);
        Body!.m_AngularVelocity = new Vector3(0, 0, 0);

        if (!Body!.m_Position.Equals(endPos))
        {
            Body!.m_Position = endPos;
        } 

        if (!Body!.m_Rotation.Equals(playerRotation))
        {
            Body.m_Rotation = playerRotation;
        }
    }

    public void Remove() {
        if (Body != null)
            Body!.Destroy();
    }
}