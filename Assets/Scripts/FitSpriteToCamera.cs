using UnityEngine;

[DefaultExecutionOrder(10000)]            // run after Cinemachine
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class BgFollowCameraSmooth : MonoBehaviour
{
    public Camera cam;
    public bool pixelPerfectSnap = true;
    public int pixelsPerUnit = 100;

    SpriteRenderer sr;
    float lastSize, lastAspect;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!cam) cam = Camera.main;
        FitNow();
    }

    void LateUpdate()
    {
        if (!cam) return;

        // follow camera AFTER it has moved this frame
        var p = cam.transform.position;

        if (pixelPerfectSnap && pixelsPerUnit > 0)
        {
            p.x = Mathf.Round(p.x * pixelsPerUnit) / pixelsPerUnit;
            p.y = Mathf.Round(p.y * pixelsPerUnit) / pixelsPerUnit;
        }

        transform.position = new Vector3(p.x, p.y, transform.position.z);

        // only recompute scale if ortho size / aspect changed
        if (Mathf.Abs(cam.orthographicSize - lastSize) > 1e-4f ||
            Mathf.Abs(cam.aspect - lastAspect) > 1e-4f)
            FitNow();
    }

    void FitNow()
    {
        if (!cam || !sr || !sr.sprite || !cam.orthographic) return;

        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;
        var spSize = sr.sprite.bounds.size;

        transform.localScale = new Vector3(worldW / spSize.x, worldH / spSize.y, 1f);
        lastSize = cam.orthographicSize;
        lastAspect = cam.aspect;
    }
}
