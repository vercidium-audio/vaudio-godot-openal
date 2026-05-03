using godot_openal;

namespace vaudio_godot_openal;

[GlobalClass]
public partial class VercidiumAudioSourceRelative : ALSource3D
{
    private VercidiumAudio vercidiumAudio;

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
            return;

        vercidiumAudio = this.GetVercidiumAudioParent();
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            return;

        Relative = true;
    }

    public override bool Play()
    {
        if (Engine.IsEditorHint())
            return false;

        // Set the effect, with no filter
        effect = vercidiumAudio.listenerReverbEffect;
        UpdateFilter(1, 1);

        return base.Play();
    }
}
