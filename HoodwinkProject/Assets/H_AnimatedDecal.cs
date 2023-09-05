using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteInEditMode]
public class H_AnimatedDecal : MonoBehaviour
{
    private DecalProjector projector;

    void Awake()
    {
        projector = GetComponent<DecalProjector>();
    }

    void LateUpdate()
    {
        projector.fadeFactor = projector.fadeFactor;
    }
}
