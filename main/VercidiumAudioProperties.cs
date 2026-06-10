namespace vaudio_godot_openal;

public partial class VercidiumAudio : Node
{
    public Node3D SceneRoot;

    [ExportGroup("World Bounds")]

    private Vector3 _position = new(-100, 0, -100);
    [Export]
    public Vector3 Position
    { 
        get => _position;
        set
        {
            _position = value;

            if (world != null)
                world.Position = ToVAudio(value);
        }
    }

    Vector3 _size = new(200, 100, 200);
    [Export]
    public Vector3 Size
    {
        get => _size;
        set
        {
            _size = value;

            if (world != null)
                world.Size = ToVAudio(value);
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

            if (world != null)
                world.MetersPerUnit = _MetersPerUnit;
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

            if (world != null)
                world.MaximumGroupedEAXCount = _maximumGroupedEAXCount;
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

            if (world != null)
                world.RenderingEnabled = value;
        }
    }
}
