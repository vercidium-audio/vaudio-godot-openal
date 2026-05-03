using System.Linq;

namespace vaudio_godot_openal;

public static class NodeExtensions
{
    public static VercidiumAudio GetVercidiumAudioParent(this Node node)
    {
        var sceneRoot = node.GetTree().CurrentScene;
        if (sceneRoot == null)
            return null;

        return sceneRoot.GetChildren().OfType<VercidiumAudio>().FirstOrDefault();
    }
}