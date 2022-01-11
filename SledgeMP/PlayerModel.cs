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

        Tags.SetTag(Body.Value, "unbreakable", "");
        // No function
        //SledgeLib.Body.SetDynamic(Body.Value, false);
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

//        SledgeLib.Body.SetTransform((uint)Body, new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0.7071068f, 0.7071068f, 0)));

        // if (Tags.HasTag(sBody.Value, "nocull")) {
        //     Tags.RemoveTag(sBody.Value, "nocull");
        //     Log.General("Removed nocull tag from player vox");
        // }

        Tags.SetTag(sBody.Value, "nocull", "");
    }

    public void Update(Vector3 startPos, Vector3 endPos, float t, Quaternion rot) {
        SledgeLib.Body.SetPosition((uint)Body!, Vector3.Lerp(startPos, endPos, 1));

        sBody!.m_LocalTransform = new Transform(new Vector3(0.3f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sHead!.m_LocalTransform = new Transform(new Vector3(0.3f, 1.4f, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sLeftArm!.m_LocalTransform = new Transform(new Vector3(-0.2f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sRightArm!.m_LocalTransform = new Transform(new Vector3(0.4f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sLeftLeg!.m_LocalTransform = new Transform(new Vector3(0, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sRightLeg!.m_LocalTransform = new Transform(new Vector3(0.3f, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));

        Tags.SetTag((uint)sBody!, "nocull", "");
        Tags.SetTag((uint)sHead!, "nocull", "");
        Tags.SetTag((uint)sLeftArm!, "nocull", "");
        Tags.SetTag((uint)sRightArm!, "nocull", "");
        Tags.SetTag((uint)sLeftLeg!, "nocull", "");
        Tags.SetTag((uint)sRightLeg!, "nocull", "");


        //SledgeLib.Body.SetRotation((uint)Body!, rot);
        //Body.m_Transform = new Transform(new Vector3(50, 10, 10), new Quaternion(0, 0, 0, 1));
        //Body.m_Rotation = Quaternion.CreateFromYawPitchRoll((float)GetPitch(camRot), 0, 0);
    }

    public void Update(Vector3 startPos, Vector3 endPos, Quaternion rot) {
        Body!.m_Position = Vector3.Lerp(startPos, endPos, 1);

        sBody!.m_LocalTransform = new Transform(new Vector3(0.3f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sHead!.m_LocalTransform = new Transform(new Vector3(0.3f, 1.4f, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sLeftArm!.m_LocalTransform = new Transform(new Vector3(-0.2f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sRightArm!.m_LocalTransform = new Transform(new Vector3(0.4f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sLeftLeg!.m_LocalTransform = new Transform(new Vector3(0, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
        sRightLeg!.m_LocalTransform = new Transform(new Vector3(0.3f, 0, 0), new Quaternion(0, 0.7071068f, 0.7071068f, 0));

        Body.m_Rotation = Quaternion.CreateFromYawPitchRoll((float)GetPitch(rot), 0, 0);
    }

    public void Remove() {
        if (Body != null)
            Log.General("Remove body - Method not implemented");
            //SledgeLib.Body.Destroy(Body.Value);
    }
}