using UnityEngine;

public class Webcam : MonoBehaviour
{
    private WebCamTexture _cam;
    private Renderer _rend;
    void Start()
    {
        _cam = new WebCamTexture(WebCamTexture.devices[0].name);
        _rend = GetComponent<Renderer>();
        _rend.material.mainTexture = _cam;
        _cam.Play();
        transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
    }
}
