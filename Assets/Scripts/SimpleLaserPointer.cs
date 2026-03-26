using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SimpleLaserPointer : MonoBehaviour
{
    public float laserLength = 10f;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.005f; 
        line.endWidth = 0.005f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.red;
        line.endColor = Color.red;
    }

    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + (transform.forward * laserLength));
    }
}