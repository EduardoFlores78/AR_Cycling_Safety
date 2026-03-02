using UnityEngine;

public class MirrorPositioner : MonoBehaviour
{
    public Transform mirrorSystem;   // The parent object
    public Transform headsetCamera;  // CenterEyeAnchor

    // Offsets: Right/Left, Up/Down, Forward/Back
    private Vector3 posHandlebar = new Vector3(0f, 0f, 0.5f);
    private Vector3 posHeadsUp = new Vector3(0f, 0.2f, 0.6f);
    private Vector3 posPeripheral = new Vector3(-0.4f, 0f, 0.6f);

    void Update()
    {
        // Lock Mirror to Headset movement
        if(mirrorSystem != null && headsetCamera != null)
        {
            mirrorSystem.SetParent(headsetCamera);
            mirrorSystem.localRotation = Quaternion.identity; // Keep it flat
        }

        // Switch Positions
        if (Input.GetKeyDown(KeyCode.Alpha1)) mirrorSystem.localPosition = posHandlebar;
        if (Input.GetKeyDown(KeyCode.Alpha2)) mirrorSystem.localPosition = posHeadsUp;
        if (Input.GetKeyDown(KeyCode.Alpha3)) mirrorSystem.localPosition = posPeripheral;
    }
}