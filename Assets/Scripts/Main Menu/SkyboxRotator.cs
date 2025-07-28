using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    public float rotateSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Skybox skybox = GetComponent<Skybox>();
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}
