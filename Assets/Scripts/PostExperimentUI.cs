using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostExperimentUI : MonoBehaviour
{

	// Update is called once per frame. On input, returns to experiment design screen.
	void Update ()
	{
		if (Input.anyKeyDown)
		{
			ExperimentController.Instance.ReturnToDesign ();
		}
	}
}
