using UnityEngine;

public class FollowBike : MonoBehaviour
{
    [Header("Attach CenterEyeAnchor Here")]
    public Transform userHead; 

    [Header("Spawner Position Relative to Bike")]
    public float sideOffset = -2.0f; // Distance to the left
    public float backOffset = -2.0f; // Distance behind you

    void Update()
    {
        if (userHead != null)
        {
            // Follow the user's physical movement, but stay locked to the floor (Y=0)
            transform.position = new Vector3(
                userHead.position.x + sideOffset, 
                0, 
                userHead.position.z + backOffset
            );

            // Keep the spawner pointing straight ahead, even if the user turns their head
            transform.rotation = Quaternion.identity; 
        }
    }
}