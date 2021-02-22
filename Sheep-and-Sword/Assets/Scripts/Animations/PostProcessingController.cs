using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
    // Variables prepared for blur effect:
    public float BlurMaxFocalLenght;
    private PostProcessVolume ppv1;
    DepthOfField dep;

    // Variables prepared for vignette effect:
    public float redVignetteMaxIntensity;
    private PostProcessVolume ppv2;
    Vignette vig;

    public void ApplyPostProcessing()
    {
        // Blur effect:
        dep = ScriptableObject.CreateInstance<DepthOfField>();
        dep.active = true;
        dep.enabled.Override(true);
        dep.aperture.Override(32.0f);
        dep.focalLength.Override(155.0f);
        dep.focusDistance.Override(1.0f);
        ppv1 = PostProcessManager.instance.QuickVolume(gameObject.layer, 0f, dep);
        StartCoroutine(IncreaseBlurIntensity());

        // Red vignette effect:
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

    // Make the blur effect more and more visible:
    IEnumerator IncreaseBlurIntensity()
    {
        while (dep.focalLength < BlurMaxFocalLenght)
        {
            dep.focalLength.value += 2;
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Make the vignette effect more and more visible:
    IEnumerator IncreaseVignetteIntensity()
    {
        while (vig.intensity < redVignetteMaxIntensity)
        {
            vig.intensity.value += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Get rid of post-processing effects after restarting the game:
    private void OnDestroy()
    {
        if (ppv1 != null) RuntimeUtilities.DestroyVolume(ppv1, true, true);
        if (ppv2 != null) RuntimeUtilities.DestroyVolume(ppv2, true, true);
    }
}
