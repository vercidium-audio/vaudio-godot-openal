using godot_openal;
using System.Linq;

namespace vaudio_godot_openal;

[Tool]
[GlobalClass]
public partial class VercidiumAudioSource : ALSource3D
{
    private VercidiumAudio vercidiumAudio;
    VercidiumAudioEmitter emitter;

    public bool Raytraced => emitter != null && emitter.Raytraced;

    private bool _PlayWhenRaytracingCompletes = true;
    private bool _RaytraceOnce = true;
    private bool _wasPlayingBeforeDeviceDestroyed = false;

    [Export]
    public bool PlayWhenRaytracingCompletes
    {
        get => _PlayWhenRaytracingCompletes;
        set => _PlayWhenRaytracingCompletes = value;
    }

    [Export]
    public bool RaytraceOnce
    {
        get => _RaytraceOnce;
        set => _RaytraceOnce = value;
    }

    [ExportGroup("Raytracing Quality")]
    
    int _ReverbRayCount = 32;
    [Export]
    public int ReverbRayCount
    {
        get => _ReverbRayCount;
        set
        {
            _ReverbRayCount = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.ReverbRayCount = _ReverbRayCount;
            }
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
            {
                emitter.ReverbBounceCount = _ReverbBounceCount;
            }
        }
    }

    int _AmbientOcclusionRayCount = 0;
    [Export]
    public int AmbientOcclusionRayCount
    {
        get => _AmbientOcclusionRayCount;
        set
        {
            _AmbientOcclusionRayCount = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.AmbientOcclusionRayCount = _AmbientOcclusionRayCount;
            }
        }
    }

    int _AmbientOcclusionBounceCount = 0;
    [Export]
    public int AmbientOcclusionBounceCount
    {
        get => _AmbientOcclusionBounceCount;
        set
        {
            _AmbientOcclusionBounceCount = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.AmbientOcclusionBounceCount = _AmbientOcclusionBounceCount;
            }
        }
    }

    int _AmbientPermeationRayCount = 0;
    [Export]
    public int AmbientPermeationRayCount
    {
        get => _AmbientPermeationRayCount;
        set
        {
            _AmbientPermeationRayCount = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.AmbientOcclusionRayCount = _AmbientPermeationRayCount;
            }
        }
    }

    int _AmbientPermeationBounceCount = 0;
    [Export]
    public int AmbientPermeationBounceCount
    {
        get => _AmbientPermeationBounceCount;
        set
        {
            _AmbientPermeationBounceCount = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.AmbientPermeationBounceCount = _AmbientPermeationBounceCount;
            }
        }
    }

    int _MaxEchogramTime = 1000;
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

    int _EchogramGranularity = 10;
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

    float _ReverbEnergyCap = 0.05f;
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float ReverbEnergyCap
    {
        get => _ReverbEnergyCap;
        set
        {
            _ReverbEnergyCap = value;

            if (emitter != null)
            {
                emitter.ReverbEnergyCap = value;
            }
        }
    }

    int _VisualisationRayCount = 0;
    [Export]
    public int VisualisationRayCount
    {
        get => _VisualisationRayCount;
        set
        {
            _VisualisationRayCount = Math.Max(0, value);

            if (emitter != null)
            {
                emitter.VisualisationRayCount = _VisualisationRayCount;
            }
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
            {
                emitter.VisualisationBounceCount = _VisualisationBounceCount;
            }
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
            {
                emitter.VisualisationUpdateFrequency = _VisualisationUpdateFrequency;
            }
        }
    }

    public override void _EnterTree()
    {
        vercidiumAudio = this.GetVercidiumAudioParent();

        if (!Engine.IsEditorHint())
        {
            // Register for a callback to re-play sounds when changing devices
            RegisterDeviceRecreatedCallback(OnDeviceRecreated);
        }

        // Must create the emitter after the parent VercidiumAudio node is initialised
        CreateEmitter();
    }

    public override string[] _GetConfigurationWarnings()
    {
        var baseWarnings = base._GetConfigurationWarnings();

        var sceneRoot = Engine.IsEditorHint() ? GetTree()?.EditedSceneRoot : GetTree()?.CurrentScene;
        if (sceneRoot == null)
            return baseWarnings;

        var vercidiumAudioNode = sceneRoot.GetChildren().OfType<VercidiumAudio>().FirstOrDefault();
        if (vercidiumAudioNode == null)
            return [.. baseWarnings, "No VercidiumAudio node found. Ensure a VercidiumAudio node exists higher up the tree."];

        // Find the top-level ancestor of this node (direct child of scene root)
        var ancestor = this as Node;
        while (ancestor.GetParent() != sceneRoot)
            ancestor = ancestor.GetParent();

        if (ancestor.GetIndex() < vercidiumAudioNode.GetIndex())
            return [.. baseWarnings, "This node must be lower in the scene tree than the VercidiumAudio node."];

        return baseWarnings;
    }

    public void CreateEmitter()
    {
        emitter = new VercidiumAudioEmitter()
        {
            Name = $"{Name}-Emitter",
            OnRaytracedByAnotherEmitterCallback = OnRaytracedByAnotherEmitter,

            // Disable all but reverb
            OcclusionRayCount = 0,
            PermeationRayCount = 0,
            AmbientOcclusionRayCount = 0,
            AmbientPermeationRayCount = 0,

            // Less rays for individual sources
            ReverbRayCount = ReverbRayCount,
            ReverbBounceCount = ReverbBounceCount,
            ReverbEnergyCap = ReverbEnergyCap,
            MaxEchogramTime = MaxEchogramTime,
            EchogramGranularity = EchogramGranularity,

            VisualisationRayCount = VisualisationRayCount,
            VisualisationBounceCount = VisualisationBounceCount,
            VisualisationUpdateFrequency = VisualisationUpdateFrequency,            
        };

        AddChild(emitter);
    }

    void OnDeviceRecreated()
    {
        // Re-play if we were playing before the device was destroyed
        if (_wasPlayingBeforeDeviceDestroyed)
        {
            _wasPlayingBeforeDeviceDestroyed = false;
            Play();
        }
    }

    void OnRaytracedByAnotherEmitter(vaudio.Emitter other)
    {
        ApplyRaytracingResults(other);

        if (PlayWhenRaytracingCompletes)
            Play();

        // Remove our emitter after we've been raytraced (this is a short sound that doesn't need continuous raytracing)
        if (RaytraceOnce)
        {
            Debug.Assert(emitter != null);

            RemoveChild(emitter);
            emitter = null;
        }
    }

    bool played = false;

    public override bool Play()
    {
        if (!Raytraced)
        {
            PlayWhenRaytracingCompletes = true;
            return false;
        }

        return played = base.Play();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Raytraced)
        {
            if (!played && PlayWhenRaytracingCompletes)
                Play();

            ApplyRaytracingResults(vercidiumAudio.listener.emitter);
        }
    }

    void ApplyRaytracingResults(vaudio.Emitter other)
    {
        effect = vercidiumAudio.GetReverbEffect(emitter);

        if (other.HasRaytracedTarget(emitter.emitter))
        {
            var vaudioFilter = other.GetTargetFilter(emitter.emitter);
            UpdateFilter(vaudioFilter.GainLF, vaudioFilter.GainHF, true);
        }
    }

    public override void OnDeviceDestroyed()
    {
        // Track if we were playing so we can re-play after device recreation
        _wasPlayingBeforeDeviceDestroyed = played && Looping;

        // Reset played state since sources are being destroyed
        played = false;

        base.OnDeviceDestroyed();
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint())
        {
            base._ExitTree();
            return;
        }

        // Unregister the device recreated callback (only registered when not in editor)
        ALManager.instance?.UnregisterDeviceRecreatedCallback(OnDeviceRecreated);

        base._ExitTree();
    }
}
