using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
    private PostProcessVolume ppv1;
    DepthOfField dep;

    private PostProcessVolume ppv2;
    Vignette vig;

    public float redVignetteMaxIntensity;
    public float BlurMaxFocalLenght;

    public void ApplyPostProcessing()
    {
        // Blur:
        dep = ScriptableObject.CreateInstance<DepthOfField>();
        dep.active = true;
        dep.enabled.Override(true);
        dep.aperture.Override(32.0f);
        dep.focalLength.Override(155.0f);
        dep.focusDistance.Override(1.0f);
        ppv1 = PostProcessManager.instance.QuickVolume(gameObject.layer, 0f, dep);
        StartCoroutine(IncreaseBlurIntensity());

        // Red vignette:
        vig = ScriptableObject.CreateInstance<Vignette>();
        vig.active = true;
        vig.enabled.Override(true);
        vig.mode.Override(VignetteMode.Classic);
        vig.color.Override(Color.red);
        vig.center.Override(new Vector2(0.5f, 0.5f));
        vig.intensity.Override(0.0f);
        vig.smoothness.Override(0.5f);
        vig.roundness.Override(1.0f);
        ppv2 = PostProcessManager.instance.QuickVolume(gameObject.layer, 0f, vig);
        StartCoroutine(IncreaseVignetteIntensity());
    }

    IEnumerator IncreaseBlurIntensity()
    {
        while (dep.focalLength < BlurMaxFocalLenght)
        {
            dep.focalLength.value += 2;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator IncreaseVignetteIntensity()
    {
        while (vig.intensity < redVignetteMaxIntensity)
        {
            vig.intensity.value += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnDestroy()
    {
        RuntimeUtilities.DestroyVolume(ppv1, true, true);
        RuntimeUtilities.DestroyVolume(ppv2, true, true);
    }
}
