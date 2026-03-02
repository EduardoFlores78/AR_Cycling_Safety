using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WifiMirrorStream : MonoBehaviour
{
    [Header("Phone Setup")]
    public string camUrl = "http://192.168.1.111:8080/shot.jpg"; 
    
    [Header("Target")]
    public Renderer mirrorScreen; 

    private Texture2D texture;
    private bool isDownloading = false;

    void Start()
    {
        // Create a texture container (starts small, resizes automatically)
        texture = new Texture2D(2, 2);
        
        if (mirrorScreen != null) 
        {
            mirrorScreen.material.mainTexture = texture;
        }
        
        StartCoroutine(FetchLoop());
    }

    IEnumerator FetchLoop()
    {
        while (true)
        {
            // Only fetch if we aren't already downloading
            if (!isDownloading) 
            {
                StartCoroutine(DownloadFrame());
            }
            yield return null; // Wait for next frame
        }
    }

    IEnumerator DownloadFrame()
    {
        isDownloading = true;
        
        // Request the image texture
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(camUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                // --- FIX IS HERE ---
                // We take the raw data bytes and load them into our texture
                texture.LoadImage(uwr.downloadHandler.data);
            }
            else
            {
                // Optional: Print error if stream fails (commented out to keep console clean)
                // Debug.LogWarning("Stream Error: " + uwr.error);
            }
        }
        
        isDownloading = false;
    }
}