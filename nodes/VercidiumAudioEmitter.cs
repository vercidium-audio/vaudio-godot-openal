using System.Linq;

namespace vaudio_godot_openal;

[Tool]
[GlobalClass]
public partial class VercidiumAudioEmitter : Node3D
{
    VercidiumAudio vercidiumAudio;
    public vaudio.Emitter emitter;

    public ALReverbEffect effect;
    public ALFilter filter;

    public float GainLF => filter?.gain ?? 0;
    public float GainHF => filter?.gainHF ?? 0;
    public bool Raytraced => emitter != null && !emitter.Initialising;

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
            return;

        vercidiumAudio = this.GetVercidiumAudioParent();

        if (vercidiumAudio == null)
        {
            VercidiumAudio.LogWarning($"Failed to initialise node {Name} because there is no VercidiumAudio node. Ensure a VercidiumAudio node exists higher up the tree");
            return;
        }

        if (!vercidiumAudio.Initialised)
        {
            VercidiumAudio.LogWarning($"Failed to initialise node {Name} because the VercidiumAudio node is not initialised yet. Ensure the VercidiumAudio node is higher up the tree");
            return;
        }

        CreateEmitter();
    }

    public override string[] _GetConfigurationWarnings()
    {
        var sceneRoot = Engine.IsEditorHint() ? GetTree()?.EditedSceneRoot : GetTree()?.CurrentScene;
        if (sceneRoot == null)
            return [];

        var vercidiumAudioNode = sceneRoot.GetChildren().OfType<VercidiumAudio>().FirstOrDefault();
        if (vercidiumAudioNode == null)
            return ["No VercidiumAudio node found. Ensure a VercidiumAudio node exists higher up the tree."];

        // Find the top-level ancestor of this node (direct child of scene root)
        var ancestor = this as Node;
        while (ancestor.GetParent() != sceneRoot)
            ancestor = ancestor.GetParent();

        if (ancestor.GetIndex() < vercidiumAudioNode.GetIndex())
            return ["This node must be lower in the scene tree than the VercidiumAudio node."];

        return [];
    }

    public void CreateEmitter()
    {
        if (emitter != null)
            throw new InvalidOperationException("Emitter already created");

        emitter = vercidiumAudio.CreateEmitter(this, OnRaytracingComplete, OnRaytracedByAnotherEmitter);
    }

    public void RemoveEmitter()
    {
        if (emitter == null)
            throw new InvalidOperationException("Emitter already removed");

        vercidiumAudio.RemoveEmitter(emitter);
        emitter = null;
    }

    void OnRaytracingComplete()
    {
        // Our own reverb and ambient permeation rays have been cast
        OnRaytracingCompleteCallback?.Invoke();
    }

    void OnRaytracedByAnotherEmitter(vaudio.Emitter emitter)
    {        
        if (GodotOpenALEnabled)
        {
            Debug.Assert(filter == null);
            filter = new(1, 1);
        }

        ApplyRaytracingResults();

        if (RaytraceOnce)
            RemoveEmitter();

        OnRaytracedByAnotherEmitterCallback?.Invoke(emitter);
    }

    public override void _Process(double delta)
    {
        // If initialisation failed, skip
        if (emitter == null)
            return;

        if (Raytraced)
            ApplyRaytracingResults();
    }

    void ApplyRaytracingResults()
    {
        effect = vercidiumAudio.GetReverbEffect(this);

        if (vercidiumAudio.listener == null)
            return;

        if (this != vercidiumAudio.listener)
        {
            if (vercidiumAudio.listener.HasRaytracedTarget(this))
            {
                var vaudioFilter = vercidiumAudio.listener.GetTargetFilter(this);
                filter?.SetGain(vaudioFilter.gainLF, vaudioFilter.gainHF);
            }
        }
    }

    public bool HasRaytracedTarget(VercidiumAudioEmitter target) => emitter.HasRaytracedTarget(target.emitter);
    public vaudio.AudioFilter GetTargetFilter(VercidiumAudioEmitter target) => emitter.GetTargetFilter(target.emitter);
    public vaudio.AudioFilter GetTargetFilter(vaudio.Emitter target) => emitter.GetTargetFilter(target);

    public override void _ExitTree()
    {
        // Remove emitter from the raytracing scene
        if (emitter != null)
            RemoveEmitter();

        base._ExitTree();
    }

    public void AddTarget(vaudio.Emitter target)
    {
        emitter.AddTarget(target);
    }

    public void RemoveTarget(vaudio.Emitter target)
    {
        emitter.RemoveTarget(target);
    }

    // Shortcuts
    public vaudio.RawReverbResults RawReverb => emitter.RawReverb;
    public vaudio.ProcessedReverbResults ProcessedReverb => emitter.ProcessedReverb;
    public vaudio.EAXReverbResults EAX => emitter.EAX;
    public float AmbientPermeationGainLF => emitter.AmbientPermeationGainLF;
    public float AmbientPermeationGainHF => emitter.AmbientPermeationGainHF;
    public int GroupedEAXIndex => emitter.GroupedEAXIndex;

    bool _IsMainListener;
    [Export]
    public bool IsMainListener
    {
        get => _IsMainListener;
        set
        {
            _IsMainListener = value;
        }
    }

    bool _RaytraceOnce;
    [Export]
    public bool RaytraceOnce
    {
        get => _RaytraceOnce;
        set
        {
            _RaytraceOnce = value;
        }
    }

    public Action OnRaytracingCompleteCallback;
    public Action<vaudio.Emitter> OnRaytracedByAnotherEmitterCallback;

    bool _AffectsGroupedEAX = true;
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

    bool _HasReverbPan = false;
    [Export]
    public bool HasReverbPan
    {
        get => _HasReverbPan;
        set
        {
            _HasReverbPan = value;

            if (emitter != null)
                emitter.HasReverbPan = value;
        }
    }

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


    [ExportGroup("Raytracing Quality")]

    int _ReverbRayCount = 128;
    [Export(PropertyHint.Range, "0,1024,1")]
    public int ReverbRayCount
    {
        get => _ReverbRayCount;
        set
        {
            _ReverbRayCount = value;

            if (emitter != null)
            {
                emitter.ReverbRayCount = value;
                emitter.ReverbEnergyCap = emitter.ReverbRayCount * emitter.ReverbBounceCount * _ReverbEnergyCap;
            }
        }
    }

    int _ReverbBounceCount = 64;
    [Export(PropertyHint.Range, "1,128,1")]
    public int ReverbBounceCount
    {
        get => _ReverbBounceCount;
        set
        {
            _ReverbBounceCount = value;

            if (emitter != null)
            {
                emitter.ReverbBounceCount = value;
                emitter.ReverbEnergyCap = emitter.ReverbRayCount * emitter.ReverbBounceCount * _ReverbEnergyCap;
            }
        }
    }


    int _MaxEchogramTime = 1000;
    [Export(PropertyHint.Range, "1,5000,1")]
    public int MaxEchogramTime
    {
        get => _MaxEchogramTime;
        set
        {
            _MaxEchogramTime = value;

            if (emitter != null)
            {
                emitter.MaxEchogramTime = value;
            }
        }
    }

    int _EchogramGranularity = 10;
    [Export(PropertyHint.Range, "1,200,1")]
    public int EchogramGranularity
    {
        get => _EchogramGranularity;
        set
        {
            _EchogramGranularity = value;

            if (emitter != null)
            {
                emitter.EchogramGranularity = value;
            }
        }
    }

    float _ReverbEnergyCap = 0.05f;
    [Export(PropertyHint.Range, "0,1")] public float ReverbEnergyCap
    { 
        get => _ReverbEnergyCap;
        set
        {
            _ReverbEnergyCap = value;

            if (emitter != null)
            {
                emitter.ReverbEnergyCap = emitter.ReverbRayCount * emitter.ReverbBounceCount * value;
            }
        }
    }

    int _OcclusionRayCount = 128;
    [Export(PropertyHint.Range, "0,1024,1")] public int OcclusionRayCount
    { 
        get => _OcclusionRayCount;
        set
        {
            _OcclusionRayCount = value;

            if (emitter != null)
                emitter.OcclusionRayCount = value;
        }
    }
    
    int _OcclusionBounceCount = 8;
    [Export(PropertyHint.Range, "1,128,1")] public int OcclusionBounceCount
    { 
        get => _OcclusionBounceCount;
        set
        {
            _OcclusionBounceCount = value;

            if (emitter != null)
                emitter.OcclusionBounceCount = value;
        }
    }

    int _PermeationRayCount = 128;
    [Export(PropertyHint.Range, "0,1024,1")] public int PermeationRayCount
    { 
        get => _PermeationRayCount;
        set
        {
            _PermeationRayCount = value;

            if (emitter != null)
                emitter.PermeationRayCount = value;
        }
    }
    
    int _PermeationBounceCount = 4;
    [Export(PropertyHint.Range, "1,128,1")] public int PermeationBounceCount
    { 
        get => _PermeationBounceCount;
        set
        {
            _PermeationBounceCount = value;

            if (emitter != null)
                emitter.PermeationBounceCount = value;
        }
    }

    int _AmbientPermeationRayCount = 128;
    [Export(PropertyHint.Range, "0,1024,1")] public int AmbientPermeationRayCount
    { 
        get => _AmbientPermeationRayCount;
        set
        {
            _AmbientPermeationRayCount = value;

            if (emitter != null)
                emitter.AmbientPermeationRayCount = value;
        }
    }
    
    int _AmbientPermeationBounceCount = 4;
    [Export(PropertyHint.Range, "1,128,1")] public int AmbientPermeationBounceCount
    { 
        get => _AmbientPermeationBounceCount;
        set
        {
            _AmbientPermeationBounceCount = value;

            if (emitter != null)
                emitter.AmbientPermeationBounceCount = value;
        }
    }

    int _VisualisationRayCount = 0;
    [Export(PropertyHint.Range, "0,1024,1")] public int VisualisationRayCount
    { 
        get => _VisualisationRayCount;
        set
        {
            _VisualisationRayCount = value;

            if (emitter != null)
                emitter.VisualisationRayCount = value;
        }
    }

    int _VisualisationBounceCount = 0;
    [Export(PropertyHint.Range, "0,128,1")]
    public int VisualisationBounceCount
    {
        get => _VisualisationBounceCount;
        set
        {
            _VisualisationBounceCount = value;

            if (emitter != null)
                emitter.VisualisationBounceCount = value;
        }
    }
    
    int _VisualisationUpdateFrequency = 500;
    [Export(PropertyHint.Range, "50,1000,50")] public int VisualisationUpdateFrequency
    { 
        get => _VisualisationUpdateFrequency;
        set
        {
            _VisualisationUpdateFrequency = value;

            if (emitter != null)
                emitter.VisualisationUpdateFrequency = value;
        }
    }
}
