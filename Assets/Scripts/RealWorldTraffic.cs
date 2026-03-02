using UnityEngine;
using System.Collections;

public class RealWorldTraffic : MonoBehaviour
{
    [Header("1. World Setup")]
    public Transform spawnPoint;        // Drag your FollowBike shadow point here!
    public Transform userHead;          // CenterEyeAnchor
    public GameObject realCarPrefab;
    public float realCarSpeed = 15.0f;  
    public float spawnInterval = 4.0f;  // Seconds to wait before spawning the NEXT car

    [Header("2. Mirror Setup")]
    public Transform mirrorLocation;    
    public GameObject mirrorCarPrefab;  
    public float heightOffset = 0.0f;   // Tweak this to get the car visible in the glass
    public float farX = 0.0f;           // Where the car starts horizontally (e.g., Outside edge)
    public float closeX = 0.2f;         // Where the car ends up horizontally (e.g., Inside edge) 
    public float carTiltX = 20.0f;      // Up/Down tilt
    public float carYawY = 180.0f;      // Left/Right spin

    [Header("3. Mirror Calibration")]
    public float maxVisibleDistance = 50.0f; 
    public float vanishDistance = 2.0f;      
    public float minScale = 0.02f;  
    public float maxScale = 0.1f;   
    public float farZ = 0.78f;      
    public float closeZ = 0.4f;     

    [Header("4. Stopping Scenario (Tailgate)")]
    public bool stopBehindUser = false; // Check box for Stop Scenes
    public float stopDistance = 5.0f;   // Real world meters before it slams the brakes

    private GameObject currentRealCar;
    private GameObject currentMirrorCar;
    private bool hasPassedUser = false;

    void Start()
    {
        StartCoroutine(TrafficLoop());
    }

    IEnumerator TrafficLoop()
    {
        // 2-second delay at the very start of the scene
        yield return new WaitForSeconds(2.0f); 

        while (true)
        {
            // Only spawn if the road is clear
            if (currentRealCar == null)
            {
                SpawnTraffic();
                hasPassedUser = false;
            }

            // If a car is parked behind the user, pause the spawner so cars don't pile up!
            if (stopBehindUser && currentRealCar != null)
            {
                float dist = Vector3.Distance(userHead.position, currentRealCar.transform.position);
                if (dist <= stopDistance + 1.0f)
                {
                    yield return new WaitForSeconds(10.0f); 
                }
            }

            // Wait a few seconds, then check if we should spawn another car
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void Update()
    {
        if (currentRealCar != null)
        {
            // FIX: Flatten the positions so height (Y) doesn't ruin the distance math!
            Vector3 flatHeadPos = new Vector3(userHead.position.x, 0, userHead.position.z);
            Vector3 flatCarPos = new Vector3(currentRealCar.transform.position.x, 0, currentRealCar.transform.position.z);

            // Calculate the flat 2D distance on the asphalt
            float distanceToUser = Vector3.Distance(flatHeadPos, flatCarPos);

            // --- A. REAL CAR MOVEMENT ---
            bool isBraking = false;
            
            // Should we slam on the brakes?
            if (stopBehindUser && distanceToUser <= stopDistance)
            {
                isBraking = true; 
            }

            // Move forward if not braking
            if (!isBraking)
            {
                currentRealCar.transform.Translate(Vector3.forward * realCarSpeed * Time.deltaTime);
            }

            // --- B. MIRROR PUPPET LOGIC ---
            if (currentMirrorCar != null)
            {
                if (!stopBehindUser && distanceToUser <= vanishDistance)
                {
                    // Car passed the vanish line! Destroy reflection.
                    Destroy(currentMirrorCar);
                    hasPassedUser = true;
                }
                else if (distanceToUser <= maxVisibleDistance && !hasPassedUser)
                {
                    // Calculate math percentage (0 to 1)
                    float percentClose = 1.0f - ((distanceToUser - vanishDistance) / (maxVisibleDistance - vanishDistance));
                    percentClose = Mathf.Clamp01(percentClose);

                    // Apply the math to the mirror puppet
                    float currentX = Mathf.Lerp(farX, closeX, percentClose); // The horizontal slide
                    float currentZ = Mathf.Lerp(farZ, closeZ, percentClose); // The depth
                    float currentScale = Mathf.Lerp(minScale, maxScale, percentClose); // The size

                    // Apply to puppet (using currentX instead of sideOffset)
                    currentMirrorCar.transform.localPosition = new Vector3(currentX, heightOffset, currentZ);
                    currentMirrorCar.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                }
            }

            // --- C. CLEANUP ---
            // Destroy the real car once it drives safely out of view ahead of the biker
            if (hasPassedUser && distanceToUser > 30.0f)
            {
                Destroy(currentRealCar);
            }
        }
    }

    public void SpawnTraffic()
    {
        currentRealCar = Instantiate(realCarPrefab, spawnPoint.position, spawnPoint.rotation);
        
        currentMirrorCar = Instantiate(mirrorCarPrefab, mirrorLocation);
        currentMirrorCar.transform.localRotation = Quaternion.Euler(carTiltX, carYawY, 0);
        
        // Force the car to be tiny and in the correct starting position instantly
        currentMirrorCar.transform.localPosition = new Vector3(farX, heightOffset, farZ);
        currentMirrorCar.transform.localScale = new Vector3(minScale, minScale, minScale);
    }
}