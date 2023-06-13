using UnityEngine;
using UnityEngine.UI;

public class SimpleTextureRotator : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float rotationSpeed = 500f;

    private void Update()
    {
        RotateTexture();
    }
    private void RotateTexture()
    {
        rawImage.rectTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
