using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RectRansformTool : MonoBehaviour {

    //public bool useWYSIWYGAnchors = true;

//    public Vector2 Test1;
//    public Vector2 Test2;

    public bool AssignAnchors = false;
//    public bool test2 = false;
//    public bool test3 = false;

    private Vector3 cachedPos;

    //public float TestWidth = 50;
    public static void SetSize(RectTransform trans, Vector2 newSize)
    {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }

	//public Transform TestObj;

	//public bool selfReposition = false;

//	void OnRectTransformDimensionsChange()
//	{
//		Debug.Log ("OnRectTransformDimensionsChange");
//		if (selfReposition == true) {
//			selfReposition = false;
//			return;
//		}
//
//		if (callCount > 10) {
//			Debug.Log ("Max calls reached");
//			return;
//		}
//
//		callCount++;
//		base.OnRectTransformDimensionsChange ();
//		RepositionAnchorsToRectCorners ();	
//
//	}



	void RepositionAnchorsToRectCorners()
	{
		Debug.Log ("Repositioning anchors---------");

		//selfReposition = true;
		RectTransform rt = GetComponent<RectTransform>();

		cachedPos = rt.transform.position;
        Debug.Log("Cached pos " + cachedPos);
		Vector2 cachedDimension = new Vector2(rt.rect.width, rt.rect.height);

		Vector3 newPos = new Vector3 (rt.rect.xMin, rt.rect.yMin, 0);

		Vector3 LeftPos = new Vector3(cachedPos.x + rt.rect.xMax, cachedPos.y + rt.rect.yMax, cachedPos.z);

		RectTransform parentRt = transform.parent.GetComponent<RectTransform> ();

		float xZero = parentRt.position.x + parentRt.rect.xMin;
		float yZero = parentRt.position.y + parentRt.rect.yMin;


		Vector2 anchorPosMin = new Vector2 (((cachedPos.x + rt.rect.xMin) - xZero ) / parentRt.rect.width, ((cachedPos.y + rt.rect.yMin ) - yZero) / parentRt.rect.height);
		Vector2 anchorPosMax = new Vector2 (((cachedPos.x + rt.rect.xMax) - xZero ) / parentRt.rect.width, ((cachedPos.y + rt.rect.yMax ) - yZero) / parentRt.rect.height);


        rt.anchorMax = anchorPosMax;
		rt.anchorMin = anchorPosMin;

		rt.transform.position = cachedPos;

		SetSize(rt, cachedDimension);
	}

    /// <summary>
    /// Notes:
    /// Use the text transform in this object as a test for where the "position" of this object is
    /// Then see if you can set the position of the text transform to the corner of this object
    /// 
    /// 
    /// The final step is to convert that co-ordinate into the anchor's space.
    /// </summary>
    /// 
    void OnValidate()
    {
//		if (useWYSIWYGAnchors == true)
//        {
//			RepositionAnchorsToRectCorners ();
//
//        }

		if (AssignAnchors == true) {
			AssignAnchors = false;

			RepositionAnchorsToRectCorners ();
		}

//        if (test3 == true)
//        {
//            test3 = false;
//           
//        }
//
//        if (test2 == true)
//        {
//			RectTransform parentRt = transform.parent.GetComponent<RectTransform> ();
//
//			if (parentRt == null) {
//				Debug.Log ("RectTransformTool: Object does not have a RectTransform parent");
//			}
//
//			RectTransform rt = GetComponent<RectTransform>();
//
//			cachedPos = rt.transform.position;
//			Vector2 cachedDimension = new Vector2(rt.rect.width, rt.rect.height);
//
//			float xZero = parentRt.position.x + parentRt.rect.xMin;
//			float yZero = parentRt.position.y + parentRt.rect.yMin;
//
//			rt.anchorMin = new Vector2 (((cachedPos.x + rt.rect.xMin) - xZero ) / parentRt.rect.width, ((cachedPos.y + rt.rect.yMin ) + yZero) / parentRt.rect.height);
//			rt.anchorMax = new Vector2 (((cachedPos.x + rt.rect.xMax) - xZero ) / parentRt.rect.width, ((cachedPos.y + rt.rect.yMax ) + yZero) / parentRt.rect.height);
//
//			// Return the box to it's original position after setting the anchor positions
//			rt.transform.position = cachedPos;
//
//			SetSize(rt, cachedDimension);
//
//            test2 = false;
//        }
    }
}

