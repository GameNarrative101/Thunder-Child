using UnityEngine;
using System.Collections;

public class ParticleBeam : MonoBehaviour
{
    PCMech pcMech;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float fadeDuration = 0.2f;
    Vector3 targetPosition;

    public void SetShooter(PCMech shooter)
    {
        this.pcMech = shooter;
    }
    void ShootBeam()
    {
        lineRenderer.useWorldSpace = true;

        lineRenderer.SetPosition(0, pcMech.GetWorldPosition());
        lineRenderer.SetPosition(1, targetPosition);
    }
    public void TryShootBeam(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        ShootBeam();
        StartCoroutine(FadeAndDestroy());
    }
    IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(0.3f);
        float timer = 0f;

        // Cache original colors
        Gradient originalGradient = lineRenderer.colorGradient;
        GradientColorKey[] colorKeys = originalGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = originalGradient.alphaKeys;

        // Start fading
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            GradientAlphaKey[] newAlphaKeys = new GradientAlphaKey[alphaKeys.Length];
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                newAlphaKeys[i] = new GradientAlphaKey(alpha, alphaKeys[i].time);
            }

            Gradient newGradient = new Gradient();
            newGradient.SetKeys(colorKeys, newAlphaKeys);
            lineRenderer.colorGradient = newGradient;

            yield return null;
        }

        Destroy(gameObject);
    }
}
