using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour {

	public SkinnedMeshRenderer MeshRenderer;

	public MovePath mMovePath;

	void Start()
	{
		if (MeshRenderer == null)
			return;

		Transform t = transform.GetChild (0);
		t.GetComponent<Animator>().CrossFade("walk", 0.1f, 0, Random.Range(0.0f, 1.0f));
		t.localRotation = Quaternion.identity;


		//SetPath ("LeftPath01");
			
		//Debug.Log ("Set animation to walk");
		

	}

	public void SetPath(WalkPath _PathObject)
	{
		mMovePath._WalkPath = _PathObject;

		if (mMovePath._WalkPath == null)
			Logger.LogError ("Walk path is missing for Person in " + name);
		
		int start = Random.Range ((int)0, (int)mMovePath._WalkPath.points.Length);
		mMovePath.startPos = transform.position;
		mMovePath.MyStart (0, start , "walk", true, true, 1);
		//mMovePath._WalkPath.DrawCurved (false);
		//mMovePath.transform.position = new Vector3(mMovePath.transform.position.x + Random.Range(0,2),mMovePath.transform.position.y, mMovePath.transform.position.z + Random.Range(0,8));

	}

	public void SetPath(WalkPath _PathObject, int _start)
	{
		mMovePath._WalkPath = _PathObject;

		if (mMovePath._WalkPath == null)
			Logger.LogError ("Walk path is missing for Person in " + name);

		int start = _start;
		mMovePath.startPos = transform.position;
		mMovePath.MyStart (0, start , "walk", true, true, 1);
		//mMovePath._WalkPath.DrawCurved (false);
		//mMovePath.transform.position = new Vector3(mMovePath.transform.position.x + Random.Range(0,2),mMovePath.transform.position.y, mMovePath.transform.position.z + Random.Range(0,8));

	}

	public void SetPath(string _pathObjectName)
	{
		Transform t = transform.GetChild (0);

		mMovePath = t.GetComponent<MovePath>();
		Debug.Log ("66666666666666 Move path = " + mMovePath + " : " + name);

		GameObject go = GameObject.Find (_pathObjectName);

		if (go != null) {
			mMovePath._WalkPath = go.GetComponent<WalkPath> ();


			if (mMovePath._WalkPath == null)
				Logger.LogError ("Walk path is missing for Person in " + name);
			int start = Random.Range ((int)0, (int)mMovePath._WalkPath.points.Length);

			mMovePath.startPos = transform.position;
			mMovePath.MyStart (0, start , "walk", true, true, 1);
			//mMovePath.MyStart (0, 10, "walk", true, true, 1);
		}
		else
			Logger.LogError ("Walk path is missing for Person in " + name + " could not find object " + _pathObjectName );
		
	}



}
