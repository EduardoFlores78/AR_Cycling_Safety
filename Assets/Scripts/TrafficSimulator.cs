using UnityEngine;
using System.Collections;

public class TrafficSimulator : MonoBehaviour
{
    [Header("1. Mirror Settings")]
    public GameObject[] mirrorCarPrefabs; 
    public Transform mirrorLocation; 

    [Header("2. Mirror Position & Tilt")]
    public float startDist = 0.78f;      
    public float heightOffset = 0.0f;   
    public float sideOffset = 0.0f;       
    public float carTilt = 20.0f;         

    [Header("3. The Perspective Trick")]
    public float startScale = 0.02f;     
    public float endScale = 0.1f;        

    [Header("4. The Handoff (Real World)")]
    public GameObject[] realCarPrefabs;  
    public Transform worldSpawnPoint;    
    public float realCarSpeed = 20.0f;   

    [Header("5. Timing")]
    public float approachSpeed = 1.0f;           
    public float vanishDist = 0.4f;      
    public float spawnInterval = 5.0f;   

    [Header("6. Stopping Scenario")]
    public bool stopBehindUser = false; // Check this box to make the car stop
    public float stopDistance = 0.5f;   // How close it gets before slamming the brakes

    private GameObject activeMirrorCar;
    private int currentCarIndex = 0; 

    void Start()
    {
        StartCoroutine(TrafficLoop());
    }

    void Update()
    {
        if (activeMirrorCar != null)
        {
            float currentZ = activeMirrorCar.transform.localPosition.z;

            // SCENARIO A: The car is supposed to stop, and it reached the stop distance.
            if (stopBehindUser && currentZ <= stopDistance)
            {
                // Do nothing! The car parks here and stares at the user.
            }
            // SCENARIO B: Normal driving (approaching)
            else
            {
                // A. Move the Mirror Car
                activeMirrorCar.transform.Translate(Vector3.forward * approachSpeed * Time.deltaTime);

                // B. Perspective Scaling
                float totalDistance = Mathf.Abs(startDist - vanishDist);
                float currentDist = Mathf.Abs(activeMirrorCar.transform.localPosition.z - startDist);
                float percent = currentDist / totalDistance;

                float currentScale = Mathf.Lerp(startScale, endScale, percent);
                activeMirrorCar.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

                // C. Check for Handoff (Only if we are NOT stopping)
                if (!stopBehindUser && activeMirrorCar.transform.localPosition.z < vanishDist)
                {
                    SpawnRealCar();
                    Destroy(activeMirrorCar);
                }
            }
        }
    }

    IEnumerator TrafficLoop()
    {
        while (true)
        {
            // If a car is parked behind the user, don't spawn a new one to crash into it!
            if (activeMirrorCar == null || !stopBehindUser)
            {
                SpawnMirrorCar();
            }
            yield return new WaitForSeconds(spawnInterval + Random.Range(0f, 2f));
        }
    }

    void SpawnMirrorCar()
    {
        if (activeMirrorCar != null) Destroy(activeMirrorCar);
        if (mirrorCarPrefabs.Length == 0) return;

        currentCarIndex = Random.Range(0, mirrorCarPrefabs.Length);
        GameObject selectedPrefab = mirrorCarPrefabs[currentCarIndex];

        activeMirrorCar = Instantiate(selectedPrefab, mirrorLocation);
        activeMirrorCar.transform.localPosition = new Vector3(sideOffset, heightOffset, startDist); 
        activeMirrorCar.transform.localRotation = Quaternion.Euler(carTilt, 180, 0);
        activeMirrorCar.transform.localScale = new Vector3(startScale, startScale, startScale); 
    }

    void SpawnRealCar()
    {
        if (realCarPrefabs.Length > currentCarIndex && worldSpawnPoint != null)
        {
            GameObject selectedPrefab = realCarPrefabs[currentCarIndex];
            GameObject realCar = Instantiate(selectedPrefab, worldSpawnPoint.position, worldSpawnPoint.rotation);
            realCar.AddComponent<MoveForward>().speed = realCarSpeed;
            Destroy(realCar, 5.0f);
        }
    }

    public class MoveForward : MonoBehaviour {
        public float speed;
        void Update() { transform.Translate(Vector3.forward * speed * Time.deltaTime); }
    }
}