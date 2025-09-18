using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10.0f;

    void Update()
    {
        Debug.Log("Update is called"); // debug
        float translationX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float translationZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        Debug.Log($"translationX: {translationX}, translationZ: {translationZ}"); // debug
        transform.Translate(translationX, 0, translationZ);
    }
}
