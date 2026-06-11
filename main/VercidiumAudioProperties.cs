namespace vaudio_godot_openal;

public partial class VercidiumAudio : Node
{
    public Node3D SceneRoot;

    [ExportGroup("World")]

    private Vector3 _Position = new(-100, 0, -100);
    [Export]
    public Vector3 Position
    { 
        get => _Position;
        set
        {
            _Position = value;

            if (world != null)
                world.Position = ToVAudio(value);
        }
    }

    Vector3 _Size = new(200, 100, 200);
    [Export]
    public Vector3 Size
    {
        get => _Size;
        set
        {
            _Size = value;

            if (world != null)
                world.Size = ToVAudio(value);
        }
    }

    float _Epsilon = 0.01f;
    [Export]
    public float Epsilon
    {
        get => _Epsilon;
        set
        {
            _Epsilon = value;

            if (world != null)
                world.Epsilon = value;
        }
    }

    bool _WorldIsIndoors = true;
    [Export]
    public bool WorldIsIndoors
    {
        get => _WorldIsIndoors;
        set
        {
            _WorldIsIndoors = value;

            if (world != null)
                world.WorldIsIndoors = value;
        }
    }

    [ExportGroup("AirAbsorption")]

    float _MetersPerUnit = 1.0f;
    [Export]
    public float MetersPerUnit
    {
        get => _MetersPerUnit;
        set
        {
            _MetersPerUnit = MathF.Max(0.0001f, value);

            if (world != null)
                world.MetersPerUnit = _MetersPerUnit;
        }
    }

    float _InverseSpeedOfSound = 1.0f / 343.0f;
    [Export]
    public float InverseSpeedOfSound
    {
        get => _InverseSpeedOfSound;
        set
        {
            _InverseSpeedOfSound = MathF.Max(0.0001f, value);

            if (world != null)
                world.InverseSpeedOfSound = _InverseSpeedOfSound;
        }
    }

    float _Humidity = 0.1f;
    [Export]
    public float Humidity
    {
        get => _Humidity;
        set
        {
            _Humidity = value;

            if (world != null)
                world.AirAbsorption.Humidity = _Humidity;
        }
    }

    float _Temperature = 26;
    [Export]
    public float Temperature
    {
        get => _Temperature;
        set
        {
            _Temperature = value;

            if (world != null)
                world.AirAbsorption.Temperature = _Temperature;
        }
    }

    float _Pressure = 101325;
    [Export]
    public float Pressure
    {
        get => _Pressure;
        set
        {
            _Pressure = value;

            if (world != null)
                world.AirAbsorption.Pressure = _Pressure;
        }
    }

    float _ReferenceFrequencyLF = 300;
    [Export]
    public float ReferenceFrequencyLF
    {
        get => _ReferenceFrequencyLF;
        set
        {
            _ReferenceFrequencyLF = MathF.Max(0.0001f, value);

            if (world != null)
                world.ReferenceFrequencyLF = _ReferenceFrequencyLF;
        }
    }

    float _ReferenceFrequencyHF = 4000.0f;
    [Export]
    public float ReferenceFrequencyHF
    {
        get => _ReferenceFrequencyHF;
        set
        {
            _ReferenceFrequencyHF = MathF.Max(0.0001f, value);

            if (world != null)
                world.ReferenceFrequencyHF = _ReferenceFrequencyHF;
        }
    }


    [ExportGroup("Emitters")]

    bool _EmittersOutsideTheWorldAreMuffled = true;
    [Export]
    public bool EmittersOutsideTheWorldAreMuffled
    {
        get => _EmittersOutsideTheWorldAreMuffled;
        set
        {
            _EmittersOutsideTheWorldAreMuffled = value;

            if (world != null)
                world.EmittersOutsideTheWorldAreMuffled = value;
        }
    }


    [ExportGroup("Threading")]

    int _MaximumConcurrencyLevel;
    [Export]
    public int MaximumConcurrencyLevel
    {
        get => _MaximumConcurrencyLevel;
        set
        {
            _MaximumConcurrencyLevel = Math.Max(0, value);

            if (world != null)
                world.MaximumConcurrencyLevel = _MaximumConcurrencyLevel;
        }
    }

    int _WorkItemCount;
    [Export]
    public int WorkItemCount
    {
        get => _WorkItemCount;
        set
        {
            _WorkItemCount = Math.Max(0, value);

            if (world != null)
                world.WorkItemCount = _WorkItemCount;
        }
    }

    bool _PendingShutdown = true;
    [Export]
    public bool PendingShutdown
    {
        get => _PendingShutdown;
        set
        {
            _PendingShutdown = value;

            if (world != null)
                world.PendingShutdown = value;
        }
    }

    [ExportGroup("EAX")]

    int _MaximumGroupedEAXCount = 3;
    [Export]
    public int MaximumGroupedEAXCount
    { 
        get => _MaximumGroupedEAXCount;
        set
        {
            _MaximumGroupedEAXCount = Math.Max(0, value);

            if (world != null)
                world.MaximumGroupedEAXCount = _MaximumGroupedEAXCount;
        }
    }

    [ExportGroup("Rendering")]

    bool _RenderingEnabled = true;
    [Export]
    public bool RenderingEnabled
    {
        get => _RenderingEnabled;
        set
        {
            _RenderingEnabled = value;

            if (world != null)
                world.RenderingEnabled = value;
        }
    }
}
