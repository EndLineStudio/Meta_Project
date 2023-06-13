using UnityEngine;
using UnityEngine.UI;

public class SimpleImageUIColorChanger : MonoBehaviour
{
    public Image targetImage;
    public float minChangeInterval = 1f;
    public float maxChangeInterval = 3f;
    public float transitionDuration = 0.5f;
    public float alpha = 1f;

    private bool isChangingColor = false;
    private Color currentColor;
    private Color targetColor;

    private void OnEnable()
    {
        isChangingColor = true;
        StartCoroutine(ChangeImageColor());
    }

    private void OnDisable()
    {
        isChangingColor = false;
    }

    private System.Collections.IEnumerator ChangeImageColor()
    {
        while (isChangingColor)
        {
            // Generate a random color with the specified alpha
            targetColor = new Color(Random.value, Random.value, Random.value, alpha);

            // Transition the color gradually
            float elapsedTime = 0f;
            Color startingColor = currentColor;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / transitionDuration);
                currentColor = Color.Lerp(startingColor, targetColor, t);
                targetImage.color = currentColor;
                yield return null;
            }

            // Apply the final color
            currentColor = targetColor;
            targetImage.color = currentColor;

            // Wait for a random interval before changing color again
            float changeInterval = Random.Range(minChangeInterval, maxChangeInterval);
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
