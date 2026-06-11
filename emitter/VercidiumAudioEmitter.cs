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
                filter?.SetGain(vaudioFilter.GainLF, vaudioFilter.GainHF);
            }
        }
    }

    public bool HasRaytracedTarget(VercidiumAudioEmitter target) => emitter.HasRaytracedTarget(target.emitter);
    public vaudio.LowPassFilter GetTargetFilter(VercidiumAudioEmitter target) => emitter.GetTargetFilter(target.emitter);
    public vaudio.LowPassFilter GetTargetFilter(vaudio.Emitter target) => emitter.GetTargetFilter(target);

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
    public vaudio.RawReverb RawReverb => emitter.RawReverb;
    public vaudio.ProcessedReverb ProcessedReverb => emitter.ProcessedReverb;
    public vaudio.EAXReverb EAX => emitter.EAX;
    public vaudio.LowPassFilter AmbientFilter => emitter.AmbientFilter;
    public int GroupedEAXIndex => emitter.GroupedEAXIndex;

    // Extra properties
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
}
