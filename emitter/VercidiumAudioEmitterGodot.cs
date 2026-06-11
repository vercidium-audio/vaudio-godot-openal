namespace vaudio_godot_openal;

public partial class VercidiumAudioEmitter : Node3D
{
    public Action OnRaytracingCompleteCallback;
    public Action<vaudio.Emitter> OnRaytracedByAnotherEmitterCallback;

    [ExportGroup("Orientation")]

    float _Pitch;
    [Export]
    public float Pitch
    {
        get => _Pitch;
        set
        {
            _Pitch = value;
        }
    }

    float _Yaw;
    [Export]
    public float Yaw
    {
        get => _Yaw;
        set
        {
            _Yaw = value;
        }
    }


    [ExportGroup("Reverb")]

    int _ReverbRayCount = 128;
    [Export]
    public int ReverbRayCount
    {
        get => _ReverbRayCount;
        set
        {
            _ReverbRayCount = Math.Max(0, value);

            if (emitter != null)
                emitter.ReverbRayCount = _ReverbRayCount;
        }
    }

    int _ReverbBounceCount = 64;
    [Export]
    public int ReverbBounceCount
    {
        get => _ReverbBounceCount;
        set
        {
            _ReverbBounceCount = Math.Max(0, value);

            if (emitter != null)
                emitter.ReverbBounceCount = _ReverbBounceCount;
        }
    }

    float _ReverbEnergyCap = 0.2f;

    [Export(PropertyHint.Range, "0.0,1.0")]
    public float ReverbEnergyCap
    {
        get => _ReverbEnergyCap;
        set
        {
            _ReverbEnergyCap = value;

            if (emitter != null)
                emitter.ReverbEnergyCap = _ReverbEnergyCap;
        }
    }

    int _MaxEchogramTime = 5000;
    [Export]
    public int MaxEchogramTime
    {
        get => _MaxEchogramTime;
        set
        {
            _MaxEchogramTime = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.MaxEchogramTime = _MaxEchogramTime;
            }
        }
    }

    int _EchogramGranularity = 50;
    [Export]
    public int EchogramGranularity
    {
        get => _EchogramGranularity;
        set
        {
            _EchogramGranularity = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.EchogramGranularity = _EchogramGranularity;
            }
        }
    }

    bool _AffectsGroupedEAX = false;
    [Export]
    public bool AffectsGroupedEAX
    {
        get => _AffectsGroupedEAX;
        set
        {
            _AffectsGroupedEAX = value;

            if (emitter != null)
                emitter.AffectsGroupedEAX = value;
        }
    }

    bool _HasRelativeReverb = true;
    [Export]
    public bool HasRelativeReverb
    {
        get => _HasRelativeReverb;
        set
        {
            _HasRelativeReverb = value;

            if (emitter != null)
                emitter.HasRelativeReverb = value;
        }
    }

    float _RelativeReverbInnerThreshold = 0.6f;
    [Export]
    public float RelativeReverbInnerThreshold
    {
        get => _RelativeReverbInnerThreshold;
        set
        {
            _RelativeReverbInnerThreshold = value;

            if (emitter != null)
                emitter.RelativeReverbInnerThreshold = value;
        }
    }

    float _RelativeReverbOuterThreshold = 0.6f;
    [Export]
    public float RelativeReverbOuterThreshold
    {
        get => _RelativeReverbOuterThreshold;
        set
        {
            _RelativeReverbOuterThreshold = value;

            if (emitter != null)
                emitter.RelativeReverbOuterThreshold = value;
        }
    }


    [ExportGroup("Muffling")]

    int _OcclusionRayCount = 256;
    [Export]
    public int OcclusionRayCount
    { 
        get => _OcclusionRayCount;
        set
        {
            _OcclusionRayCount = Math.Max(0, value);

            if (emitter != null)
                emitter.OcclusionRayCount = _OcclusionRayCount;
        }
    }
    
    int _OcclusionBounceCount = 8;
    [Export]
    public int OcclusionBounceCount
    { 
        get => _OcclusionBounceCount;
        set
        {
            _OcclusionBounceCount = Math.Max(0, value);

            if (emitter != null)
                emitter.OcclusionBounceCount = _OcclusionBounceCount;
        }
    }
    
    float _OcclusionEnergyCap = 0.15f;
    [Export]
    public float OcclusionEnergyCap
    { 
        get => _OcclusionEnergyCap;
        set
        {
            _OcclusionEnergyCap = Math.Max(0, value);

            if (emitter != null)
                emitter.OcclusionEnergyCap = _OcclusionEnergyCap;
        }
    }

    int _PermeationRayCount = 128;
    [Export]
    public int PermeationRayCount
    { 
        get => _PermeationRayCount;
        set
        {
            _PermeationRayCount = Math.Max(0, value);

            if (emitter != null)
                emitter.PermeationRayCount = _PermeationRayCount;
        }
    }
    
    int _PermeationBounceCount = 2;
    [Export]
    public int PermeationBounceCount
    { 
        get => _PermeationBounceCount;
        set
        {
            _PermeationBounceCount = Math.Max(0, value);

            if (emitter != null)
                emitter.PermeationBounceCount = _PermeationBounceCount;
        }
    }

    float _PermeationEnergyCap = 0.15f;
    [Export]
    public float PermeationEnergyCap
    {
        get => _PermeationEnergyCap;
        set
        {
            _PermeationEnergyCap = Math.Max(0, value);

            if (emitter != null)
                emitter.PermeationEnergyCap = _PermeationEnergyCap;
        }
    }


    [ExportGroup("Ambience")]

    int _AmbientOcclusionRayCount = 128;
    [Export]
    public int AmbientOcclusionRayCount
    { 
        get => _AmbientOcclusionRayCount;
        set
        {
            _AmbientOcclusionRayCount = Math.Max(0, value);

            if (emitter != null)
                emitter.AmbientOcclusionRayCount = _AmbientOcclusionRayCount;
        }
    }
    
    int _AmbientOcclusionBounceCount = 4;
    [Export]
    public int AmbientOcclusionBounceCount
    { 
        get => _AmbientOcclusionBounceCount;
        set
        {
            _AmbientOcclusionBounceCount = Math.Max(0, value);

            if (emitter != null)
                emitter.AmbientOcclusionBounceCount = _AmbientOcclusionBounceCount;
        }
    }

    float _AmbientOcclusionEnergyCap = 0.15f;
    [Export]
    public float AmbientOcclusionEnergyCap
    {
        get => _AmbientOcclusionEnergyCap;
        set
        {
            _AmbientOcclusionEnergyCap = Math.Max(0, value);

            if (emitter != null)
                emitter.AmbientOcclusionEnergyCap = _AmbientOcclusionEnergyCap;
        }
    }

    int _AmbientPermeationRayCount = 128;
    [Export]
    public int AmbientPermeationRayCount
    { 
        get => _AmbientPermeationRayCount;
        set
        {
            _AmbientPermeationRayCount = Math.Max(0, value);

            if (emitter != null)
                emitter.AmbientPermeationRayCount = _AmbientPermeationRayCount;
        }
    }
    
    int _AmbientPermeationBounceCount = 2;
    [Export]
    public int AmbientPermeationBounceCount
    { 
        get => _AmbientPermeationBounceCount;
        set
        {
            _AmbientPermeationBounceCount = Math.Max(0, value);

            if (emitter != null)
                emitter.AmbientPermeationBounceCount = _AmbientPermeationBounceCount;
        }
    }

    float _AmbientPermeationEnergyCap = 0.15f;
    [Export]
    public float AmbientPermeationEnergyCap
    {
        get => _AmbientPermeationEnergyCap;
        set
        {
            _AmbientPermeationEnergyCap = Math.Max(0, value);

            if (emitter != null)
                emitter.AmbientPermeationEnergyCap = _AmbientPermeationEnergyCap;
        }
    }

    [ExportGroup("Visualisation")]

    int _VisualisationRayCount = 0;
    [Export]
    public int VisualisationRayCount
    { 
        get => _VisualisationRayCount;
        set
        {
            _VisualisationRayCount = Math.Max(0, value);

            if (emitter != null)
                emitter.VisualisationRayCount = _VisualisationRayCount;
        }
    }

    int _VisualisationBounceCount = 0;
    [Export]
    public int VisualisationBounceCount
    {
        get => _VisualisationBounceCount;
        set
        {
            _VisualisationBounceCount = Math.Max(0, value);

            if (emitter != null)
                emitter.VisualisationBounceCount = _VisualisationBounceCount;
        }
    }
    
    int _VisualisationUpdateFrequency = 500;
    [Export]
    public int VisualisationUpdateFrequency
    { 
        get => _VisualisationUpdateFrequency;
        set
        {
            _VisualisationUpdateFrequency = Math.Max(0, value);

            if (emitter != null)
                emitter.VisualisationUpdateFrequency = _VisualisationUpdateFrequency;
        }
    }


    [ExportGroup("Debug Rendering")]

    Godot.Color _TrailColor = new(1, 1, 1, 0.1f);
    [Export]
    public Godot.Color TrailColor
    {
        get => _TrailColor;
        set
        {
            _TrailColor = value;

            if (emitter != null)
                emitter.TrailColor = ToVAudio(value);
        }
    }

    Godot.Color _ReverbColor = new(27.0f / 255.0f, 247.0f / 255.0f, 255.0f / 255.0f, 0.2f);
    [Export]
    public Godot.Color ReverbColor
    {
        get => _ReverbColor;
        set
        {
            _ReverbColor = value;

            if (emitter != null)
                emitter.ReverbColor = ToVAudio(value);
        }
    }

    Godot.Color _OcclusionColor = new(27.0f / 255.0f, 247.0f / 255.0f, 255.0f / 255.0f, 0.2f);
    [Export]
    public Godot.Color OcclusionColor
    {
        get => _OcclusionColor;
        set
        {
            _OcclusionColor = value;

            if (emitter != null)
                emitter.OcclusionColor = ToVAudio(value);
        }
    }

    Godot.Color _PermeationColor = new(255.0f / 255.0f, 127.0f / 255.0f, 42.0f / 255.0f, 0.2f);
    [Export]
    public Godot.Color PermeationColor
    {
        get => _PermeationColor;
        set
        {
            _PermeationColor = value;

            if (emitter != null)
                emitter.PermeationColor = ToVAudio(value);
        }
    }

    Godot.Color _AmbientPermeationColor = new(255.0f / 255.0f, 204.0f / 255.0f, 0.0f, 0.2f);
    [Export]
    public Godot.Color AmbientPermeationColor
    {
        get => _AmbientPermeationColor;
        set
        {
            _AmbientPermeationColor = value;

            if (emitter != null)
                emitter.AmbientPermeationColor = ToVAudio(value);
        }
    }

    [ExportGroup("Advanced")]

    int _Type;
    [Export]
    public int Type
    {
        get => _Type;
        set
        {
            _Type = value;

            if (emitter != null)
                emitter.Type = value;
        }
    }

    int _ReservedEmitterTargets;
    [Export]
    public int ReservedEmitterTargets
    {
        get => _ReservedEmitterTargets;
        set
        {
            _ReservedEmitterTargets = value;

            if (emitter != null)
                emitter.ReservedEmitterTargets = value;
        }
    }

    int _RefreshRayCount = 16;
    [Export]
    public int RefreshRayCount
    {
        get => _RefreshRayCount;
        set
        {
            _RefreshRayCount = value;

            if (emitter != null)
                emitter.RefreshRayCount = value;
        }
    }

    float _RefreshDistanceThreshold = 1.0f;
    [Export]
    public float RefreshDistanceThreshold
    {
        get => _RefreshDistanceThreshold;
        set
        {
            _RefreshDistanceThreshold = value;

            if (emitter != null)
                emitter.RefreshDistanceThreshold = value;
        }
    }

    int _ScatteringSeed = Random.Shared.Next();
    [Export]
    public int ScatteringSeed
    {
        get => _ScatteringSeed;
        set
        {
            _ScatteringSeed = value;

            if (emitter != null)
                emitter.RefreshRayCount = value;
        }
    }

    bool _ClampPosition = true;
    [Export]
    public bool ClampPosition
    {
        get => _ClampPosition;
        set
        {
            _ClampPosition = value;

            if (emitter != null)
                emitter.ClampPosition = value;
        }
    }
}
