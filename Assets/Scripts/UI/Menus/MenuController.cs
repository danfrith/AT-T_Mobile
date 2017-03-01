using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

    static public MenuController _instance;

	public MenuBase[] Menus;
	
	public MenuBase CurrentMenu;
    public GameObject hudHolder;

   void Awake()
    {
        _instance = this;
    }
    
	public void OnEnable()
	{
		if (CurrentMenu != null)
		{
			CurrentMenu.Show();
		}
		
		Menus = gameObject.GetComponentsInChildren<MenuBase>();
	}
	
	public void OpenMenu(MenuBase _menu)
	{
		for (int i = 0; i < Menus.Length; i++)
		{
			if (Menus[i] != _menu)
			{	
			
			
				Menus[i].Hide();
				//Debug.Log("Closing menu " + Menus[i].gameObject.name);
			}
			else 
				_menu.Show();
			
			CurrentMenu = _menu;
		}
	}
	
	public void CloseCurrentMenu()
	{
		if (CurrentMenu != null)
			CurrentMenu.Hide();
		
		CurrentMenu = null;
	}


 
}
