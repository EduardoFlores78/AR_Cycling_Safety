using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuController : MonoBehaviour
{
    // --- 1. OVERTAKING SCENARIOS ---
    public void LoadRearView_Overtake()
    {
        SceneManager.LoadScene("RearView"); 
    }

    public void LoadSideMirror_Overtake()
    {
        SceneManager.LoadScene("SideMirror"); 
    }

    // --- 2. STOPPING SCENARIOS ---
    public void LoadRearView_Stop()
    {
        SceneManager.LoadScene("RearView-Stop"); 
    }

    public void LoadSideMirror_Stop()
    {
        SceneManager.LoadScene("SideMirror-Stop"); 
    }
}