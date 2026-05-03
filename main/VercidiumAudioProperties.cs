namespace vaudio_godot_openal;

public partial class VercidiumAudio : Node
{
    public Node3D SceneRoot;

    [ExportGroup("World Bounds")]

    private Vector3 _worldPosition = new(-100, 0, -100);
    [Export]
    public Vector3 WorldPosition
    { 
        get => _worldPosition;
        set
        {
            _worldPosition = value;

            if (context != null)
                context.WorldPosition = ToVAudio(value);
        }
    }

    Vector3 _worldSize = new(200, 100, 200);
    [Export]
    public Vector3 WorldSize
    {
        get => _worldSize;
        set
        {
            _worldSize = value;

            if (context != null)
                context.WorldSize = ToVAudio(value);
        }
    }

    float _MetersPerUnit = 1;
    [Export]
    public float MetersPerUnit
    { 
        get => _MetersPerUnit;
        set
        {
            _MetersPerUnit = MathF.Max(0, value);

            if (context != null)
                context.MetersPerUnit = _MetersPerUnit;
        }
    }


    [ExportGroup("EAX")]

    int _maximumGroupedEAXCount = 3;
    [Export]
    public int MaximumGroupedEAXCount
    { 
        get => _maximumGroupedEAXCount;
        set
        {
            _maximumGroupedEAXCount = Math.Max(0, value);

            if (context != null)
                context.MaximumGroupedEAXCount = _maximumGroupedEAXCount;
        }
    }


    [ExportGroup("Rendering")]

    bool _renderingEnabled = true;
    [Export]
    public bool RenderingEnabled
    {
        get => _renderingEnabled;
        set
        {
            _renderingEnabled = value;

            if (context != null)
                context.RenderingEnabled = value;
        }
    }
}
