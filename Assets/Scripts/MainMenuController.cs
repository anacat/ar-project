using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadObjectPlacement()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMultipleImageTracking()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadMultipleObjectPlacement()
    {
        SceneManager.LoadScene(3);
    }
    
    private void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }
}
