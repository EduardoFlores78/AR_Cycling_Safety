using UnityEngine;
using System.Collections;

public class RealWorldTraffic : MonoBehaviour
{
    [Header("1. World Setup")]
    public Transform spawnPoint;        
    public Transform userHead;          
    public GameObject realCarPrefab;
    public float realCarSpeed = 15.0f;  
    public float spawnInterval = 4.0f;  

    [Header("2. Mirror Setup")]
    public Transform mirrorLocation;    
    public GameObject mirrorCarPrefab;  
    public float heightOffset = 0.0f;   
    public float farX = 0.0f;           
    public float closeX = 0.2f;         
    public float carTiltX = 20.0f;      
    public float carYawY = 180.0f;      

    [Header("3. Mirror Calibration")]
    public float maxVisibleDistance = 50.0f; 
    public float vanishDistance = 2.0f;      
    public float minScale = 0.02f;  
    public float maxScale = 0.1f;   
    public float farZ = 0.78f;      
    public float closeZ = 0.4f;     

    [Header("4. Stopping Scenario (Tailgate)")]
    public bool stopBehindUser = false; 
    public float stopDistance = 5.0f;   

    [Header("5. Radar Audio System")]
    public AudioSource radarAudioSource;   
    public AudioClip beepSound;            
    public float radarStartDistance = 30.0f; 
    public float radarMaxDangerDist = 5.0f;  
    public float slowBeepInterval = 1.0f;    
    public float fastBeepInterval = 0.1f;    

    [Header("6. Haptic Feedback (Vibration)")]
    public bool enableHaptics = true;
    [Range(0f, 1f)] public float hapticAmplitude = 0.8f; // How hard it vibrates
    [Range(0f, 1f)] public float hapticFrequency = 0.5f; // The "pitch" of the vibration
    public float hapticDuration = 0.1f;                  // How long the pulse lasts (seconds)

    private GameObject currentRealCar;
    private GameObject currentMirrorCar;
    private bool hasPassedUser = false;
    private float beepTimer = 0.0f; 

    void Start()
    {
        StartCoroutine(TrafficLoop());
    }

    IEnumerator TrafficLoop()
    {
        yield return new WaitForSeconds(2.0f); 

        while (true)
        {
            if (currentRealCar == null)
            {
                SpawnTraffic();
                hasPassedUser = false;
                beepTimer = 0.0f; 
            }

            if (stopBehindUser && currentRealCar != null)
            {
                Vector3 flatHead = new Vector3(userHead.position.x, 0, userHead.position.z);
                Vector3 flatCar = new Vector3(currentRealCar.transform.position.x, 0, currentRealCar.transform.position.z);
                if (Vector3.Distance(flatHead, flatCar) <= stopDistance + 1.0f)
                {
                    yield return new WaitForSeconds(10.0f); 
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void Update()
    {
        if (currentRealCar != null)
        {
            Vector3 flatHeadPos = new Vector3(userHead.position.x, 0, userHead.position.z);
            Vector3 flatCarPos = new Vector3(currentRealCar.transform.position.x, 0, currentRealCar.transform.position.z);
            float distanceToUser = Vector3.Distance(flatHeadPos, flatCarPos);

            // --- A. REAL CAR MOVEMENT ---
            bool isBraking = false;
            if (stopBehindUser && distanceToUser <= stopDistance)
            {
                isBraking = true; 
            }

            if (!isBraking)
            {
                currentRealCar.transform.Translate(Vector3.forward * realCarSpeed * Time.deltaTime);
            }

            // --- B. MIRROR PUPPET LOGIC ---
            if (currentMirrorCar != null)
            {
                if (!stopBehindUser && distanceToUser <= vanishDistance)
                {
                    Destroy(currentMirrorCar);
                    hasPassedUser = true;
                }
                else if (distanceToUser <= maxVisibleDistance && !hasPassedUser)
                {
                    float percentClose = 1.0f - ((distanceToUser - vanishDistance) / (maxVisibleDistance - vanishDistance));
                    percentClose = Mathf.Clamp01(percentClose);

                    float currentX = Mathf.Lerp(farX, closeX, percentClose); 
                    float currentZ = Mathf.Lerp(farZ, closeZ, percentClose); 
                    float currentScale = Mathf.Lerp(minScale, maxScale, percentClose); 

                    currentMirrorCar.transform.localPosition = new Vector3(currentX, heightOffset, currentZ);
                    currentMirrorCar.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                }
            }

            // --- C. RADAR AUDIO & HAPTIC LOGIC ---
            if (radarAudioSource != null && beepSound != null && !hasPassedUser)
            {
                if (distanceToUser <= radarStartDistance)
                {
                    beepTimer += Time.deltaTime;

                    float radarPercent = 1.0f - ((distanceToUser - radarMaxDangerDist) / (radarStartDistance - radarMaxDangerDist));
                    radarPercent = Mathf.Clamp01(radarPercent);

                    float currentBeepInterval = Mathf.Lerp(slowBeepInterval, fastBeepInterval, radarPercent);

                    if (beepTimer >= currentBeepInterval)
                    {
                        // 1. Play the beep
                        radarAudioSource.PlayOneShot(beepSound);
                        
                        // 2. Trigger the synced vibration
                        if (enableHaptics)
                        {
                            StartCoroutine(TriggerHapticPulse());
                        }

                        beepTimer = 0.0f;
                    }
                }
            }

            // --- D. CLEANUP ---
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
        
        currentMirrorCar.transform.localPosition = new Vector3(farX, heightOffset, farZ);
        currentMirrorCar.transform.localScale = new Vector3(minScale, minScale, minScale);
    }

   
    IEnumerator TriggerHapticPulse()
    {
        // Turn ON vibration for BOTH controllers (so it doesn't matter which arm you strap it to)
        OVRInput.SetControllerVibration(hapticFrequency, hapticAmplitude, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(hapticFrequency, hapticAmplitude, OVRInput.Controller.RTouch);

        // Wait for a split second (0.1s by default)
        yield return new WaitForSeconds(hapticDuration);

        // Turn OFF vibration
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
    
    // Safety fallback: if the script is disabled, ensure controllers stop vibrating
    void OnDisable()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
}