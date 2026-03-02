using UnityEngine;
using System.Collections;

public class SimpleLaneSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject carPrefab;    // Drag your car here
    public float carSpeed = 10f;    // Speed in meters/sec (10 = ~22mph)
    public float spawnInterval = 3f; // Seconds between cars
    public float destroyTime = 10f; // How long until car deletes itself

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOneCar();
            // Randomize the interval slightly so it looks natural
            float randomWait = Random.Range(spawnInterval - 1f, spawnInterval + 1f);
            yield return new WaitForSeconds(randomWait);
        }
    }

    void SpawnOneCar()
    {
        // 1. Create the car at THIS object's position and rotation
        GameObject newCar = Instantiate(carPrefab, transform.position, transform.rotation);
        
        // 2. Add a temporary "Driver" component to move it
        // We add this continuously so we don't need a separate script for the car prefab
        newCar.AddComponent<MoveForward>().speed = carSpeed;
        
        // 3. Destroy it after X seconds (so your game doesn't lag forever)
        Destroy(newCar, destroyTime);
    }

    // This mini-class handles the actual movement
    public class MoveForward : MonoBehaviour
    {
        public float speed;
        void Update()
        {
            // Move strictly "Forward" relative to where the car is facing
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}