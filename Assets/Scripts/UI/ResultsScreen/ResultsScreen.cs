using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResultsScreen : MonoBehaviour {

	public Transform ResultWindow;

	public Transform ResultItem;

	void OnEnable()
	{
        ClearItems();
        SpawnItems ();
	}

	private List<ResultsScreenItem> Items;

    void ClearItems()
    {
        for (int i = 0; i < ResultWindow.transform.childCount; i++)
        {
            Destroy(ResultWindow.transform.GetChild(i).gameObject);
        }
    }

	void SpawnItems()
	{
		if (Items != null)
			return;
		
		Items = new List<ResultsScreenItem> ();

		for (int i = 0; i < 6; i++) { ///<<< noooooo
			Items.Add(InstantiateResultItem(i));
		}
	}

	ResultsScreenItem InstantiateResultItem(int _id)
	{
		VirusType type = (VirusType)(_id);

		Transform t = (Transform)Instantiate (ResultItem, ResultWindow);

		t.localScale = Vector3.one;
		RectTransform ItemRect  = t.GetComponent<RectTransform> ();
		//ItemRect.sizeDelta = new Vector2(containerRect.rect.width, containerRect.rect.height);
		ItemRect.localScale = Vector3.one;
        ItemRect.localPosition = new Vector3(ItemRect.localPosition.x, ItemRect.localPosition.y, 0);

        ItemRect.localRotation = Quaternion.identity;

		ResultsScreenItem r = t.gameObject.GetComponent<ResultsScreenItem> ();

		if (r == null) {
			Logger.LogError ("ResultScreenItem is missing for spawned item " + _id);
			return null;
		}

		r.Init (type);

		return r;
	}


}
