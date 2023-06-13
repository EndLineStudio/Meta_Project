using System.Collections;
using UnityEngine;

public class SimpleCameraChange : MonoBehaviour
{
    [SerializeField]
    private Transform cameraHandle;

    [SerializeField]
    private Vector3 originalView;

    [SerializeField]
    private Vector3 closeView;

    [SerializeField, Range(0.1f, 2.0f)]
    private float transitionDuration = 0.5f;

    private Vector3 currentView;
    private Vector3 targetView;
    private Coroutine transitionCoroutine;

    private void Start()
    {
        currentView = originalView;
        ApplyCameraView();
    }
    public void SetCameraCloseView()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        targetView = closeView;
        transitionCoroutine = StartCoroutine(TransitionCameraView());
    }
    public void SetCameraOriginalView()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        targetView = originalView;
        transitionCoroutine = StartCoroutine(TransitionCameraView());
    }
    private IEnumerator TransitionCameraView()
    {
        float elapsedTime = 0f;
        Vector3 startingView = currentView;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            currentView = Vector3.Lerp(startingView, targetView, t);
            ApplyCameraView();

            yield return null;
        }

        currentView = targetView;
        ApplyCameraView();

        transitionCoroutine = null;
    }
    private void ApplyCameraView()
    {
        if (cameraHandle != null)
        {
            cameraHandle.localPosition = currentView;
        }
    }
}
