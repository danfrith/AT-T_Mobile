using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MenuBase))]
public class OpenMenuOnKey : MonoBehaviour {

    public KeyCode MenuKey;

    private MenuBase mMenu;
    void OnEnable()
    {
        mMenu = GetComponent<MenuBase>();
    }

	void Update () {

        if (Input.GetKeyDown(MenuKey))
        {
            if (mMenu.MenuOpen)
                mMenu.Hide();
            else
                mMenu.Show();
        }
	
	}
}
