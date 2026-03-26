using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuController : MonoBehaviour
{

    public void LoadRearView_Haptic()
    {
        SceneManager.LoadScene("RearView-Haptic"); 
    }

    public void LoadSideMirror_Haptic()
    {
        SceneManager.LoadScene("SideMirror-Haptic"); 
    }

    public void LoadRearView_Sound()
    {
        SceneManager.LoadScene("RearView-Sound"); 
    }

    public void LoadSideMirror_Sound()
    {
        SceneManager.LoadScene("SideMirror-Sound"); 
    }
    public void LoadRearView_Both()
    {
        SceneManager.LoadScene("RearView-Both"); 
    }

    public void LoadSideMirror_Both()
    {
        SceneManager.LoadScene("SideMirror-Both"); 
    }
}