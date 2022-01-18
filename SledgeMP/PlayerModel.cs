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

    public CBody? ToolBody = null;
    public CShape? ToolShape = null;

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

    public void CreatePlayerTool()
    {
        ToolBody = new CBody();
        ToolShape = new CShape(ToolBody);
        ToolBody.m_Dynamic = false;
        Tags.SetTag(ToolBody.m_Handle, "unbreakable", "");
    }

    public void Load() {
        if (Body == null || sBody == null || sHead == null || sLeftArm == null || sRightArm == null || sLeftLeg == null || sRightLeg == null || ToolBody == null || ToolShape == null)
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

        // Player Tool

        CShape.LoadVox(ToolShape.m_Handle, "Assets/Models/sledge.vox", "", 0.5f);
        ToolBody.m_Transform = new Transform(new Vector3(-0.2f, 0.7f, 0.1f), new Quaternion(0, 0.7071068f, 0.7071068f, 0));
    }

    public void Update(Vector3 startPos, Vector3 endPos, Quaternion playerRotation) {
        if (Body == null || ToolBody == null)
            return;

        Body.m_Velocity = new Vector3(0, 0, 0);
        Body.m_AngularVelocity = new Vector3(0, 0, 0);

        ToolBody.m_Velocity = new Vector3(0, 0, 0);
        ToolBody.m_AngularVelocity = new Vector3(0, 0, 0);

        if (!Body.m_Position.Equals(endPos))
        {
            Body.m_Position = endPos;
            ToolBody.m_Transform = sLeftArm!.m_LocalTransform;
            ToolBody.m_Position = Vector3.Add(Body!.m_Position, sLeftArm!.m_LocalTransform.Position); // TODO: Tool rotations!
        } 

        if (!Body.m_Rotation.Equals(playerRotation))
        {
            Body.m_Rotation = playerRotation;
        }
    }

    public void UpdateTool(string newTool)
    {
        ToolBody!.Destroy();
        CreatePlayerTool();

        var T = new ModelSpawner
        {
            m_iHandle = ToolShape!.m_Handle,
            m_VoxPath = "Assets/Models/" + newTool + ".vox"
        };

        Client.m_ModelsToLoad.Add(T);
     
    }

    public void Remove() {
        if (Body != null)
            Body!.Destroy();
    }
}