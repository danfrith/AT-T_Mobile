using UnityEngine;
using System.Collections;

public class PauseScreen : MonoBehaviour {

    public MenuBase PauseMenu;

    // this is a last minute addition, sorry for the hackeyness. 
    public MenuBase SettingScreen;

    public void ReturnButtonPressed()
    {
        if (PauseMenu == null)
        {
            Debug.LogError("Menu base is null");
            return;
        }

        MobileGameManager.Instance.Resume();
        PauseMenu.Hide();
        
    }

    public void HomeButtonPressed()
    {
        MobileGameManager.Instance.Resume();
        MobileGameManager.Instance.StageEnded(false, 0);
    }

    public void RestartLevel()
    {
        MobileGameManager.Instance.Resume();
        MobileGameManager.Instance.RestartLevel();
    }

    public void PausebuttonPresed()
    {
        MobileGameManager.Instance.Pause();
        PauseMenu.Show();
    }

    // read SettingsScreenBackButtonPressed() note
    public void SettingsButtonPressed()
    {
        Debug.Log("Settings button pressed");
        SettingScreen.Show();
        PauseMenu.Hide(); 
    }

    // The settings screen will act like a sub menu to this menu 
    // pressing back on the settins screen will re-open the pause screen
    public void SettingsScreenBackButtonPressed()
    {
        SettingScreen.Hide();
        PauseMenu.Show();
    }

}
