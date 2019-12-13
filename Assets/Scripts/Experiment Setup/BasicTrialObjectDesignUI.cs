/******************************************************************************
 * File: BasicTrialObjectDesignUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the BasicTrialObjectDesignUI class. This class
 * has a display of TrialObjects that will be used in the test.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

[RequireComponent (typeof(Image))]
public class BasicTrialObjectDesignUI : MonoBehaviour, IValidatedUI, IHighlightableUI
{
	public Dropdown ObjectSelectionDropdown;
	public VerticalLayoutGroup ObjectDisplayLayout;

	public BounceObjectUI BouncingObjectUIPrefab;
	public UnusualObjectUI UnusualObjectUIPrefab;

	public Color NormalColor, HighlightedColor;

	public BasicTrialManager.BouncingObjectSettings BouncingObjectDefaultSettings;
	public BasicTrialManager.UnusualObjectSettings UnusualObjectDefaultSettings;

	private List<BounceObjectUI> BounceObjectList;
	private List<UnusualObjectUI> UnusualObjectList;

	private Image Background;

	/// <summary>
	/// Initializes object lists if they haven't been set up.
	/// </summary>
	private void Awake ()
	{
		if (BounceObjectList == null)
		{
			BounceObjectList = new List<BounceObjectUI> ();
		}
		if (UnusualObjectList == null)
		{
			UnusualObjectList = new List<UnusualObjectUI> ();
		}

		Background = GetComponent<Image> ();
	}

	/// <summary>
	/// Adds a trial object depending on the current value of the dropdown box.
	/// </summary>
	public void AddTrialObject ()
	{


		switch (ObjectSelectionDropdown.value)
		{
			case (int)TrialObject.ObjectType.BounceObject:
				AddTrialObject (BouncingObjectDefaultSettings);
				StartCoroutine (ScrollList ());
				break;
			case (int)TrialObject.ObjectType.UnusualObject:
				AddTrialObject (UnusualObjectDefaultSettings);
				StartCoroutine (ScrollList ());
				break;
		}
	}

	/// <summary>
	/// Waits for the next frame before scrolling the display list to show the most recently added object.
	/// </summary>
	public IEnumerator ScrollList ()
	{
		yield return null;
		RectTransform displayTransform = ObjectDisplayLayout.GetComponent<RectTransform> ();
		displayTransform.localPosition = new Vector2 (displayTransform.localPosition.x, displayTransform.rect.height);
	}

	/// <summary>
	/// Adds a BounceObjectUI to the display based on the passed in BouncingObjectSettings.
	/// </summary>
	/// <param name="bouncingObject">The object to add to the UI display.</param>
	public void AddTrialObject (BasicTrialManager.BouncingObjectSettings bouncingObject)
	{
		if (BounceObjectList == null)
		{
			BounceObjectList = new List<BounceObjectUI> ();
		}
		BounceObjectUI bounceUI = GameObject.Instantiate<BounceObjectUI> (BouncingObjectUIPrefab);
		bounceUI.InitializeUI (this, bouncingObject);
		bounceUI.GetComponent<RectTransform> ().SetParent (ObjectDisplayLayout.transform, false);
		BounceObjectList.Add (bounceUI);
	}

	/// <summary>
	/// Adds a UnusualObjectUI to the display based on the passed in UnusualObjectSettings.
	/// </summary>
	/// <param name="unusualObject">The object to add to the UI display.</param>
	public void AddTrialObject (BasicTrialManager.UnusualObjectSettings unusualObject)
	{
		if (UnusualObjectList == null)
		{
			UnusualObjectList = new List<UnusualObjectUI> ();
		}
		UnusualObjectUI unusualUI = GameObject.Instantiate<UnusualObjectUI> (UnusualObjectUIPrefab);
		unusualUI.InitializeUI (this, unusualObject);
		unusualUI.GetComponent<RectTransform> ().SetParent (ObjectDisplayLayout.transform, false);
		UnusualObjectList.Add (unusualUI);
	}

	/// <summary>
	/// Removes the passed in BounceObjectUI from the display and the test setup.
	/// </summary>
	/// <param name="trialObject">The object to remove.</param>
	public void RemoveTrialObject (BounceObjectUI trialObject)
	{
		foreach (BounceObjectUI ui in BounceObjectList)
		{
			if (ui.Equals (trialObject))
			{
				Destroy (ui.gameObject);
				BounceObjectList.Remove (ui);
				break;
			}
		}
	}
	
	/// <summary>
	/// Removes the passed in UnusualObjectUI from the display and the test setup.
	/// </summary>
	/// <param name="trialObject">The object to remove.</param>
	public void RemoveTrialObject (UnusualObjectUI trialObject)
	{ 
		foreach (UnusualObjectUI ui in UnusualObjectList)
		{
			if (ui.Equals (trialObject))
			{
				Destroy (ui.gameObject);
				UnusualObjectList.Remove (ui);
				break;
			}
		}
	}

	/// <summary>
	/// Unifys the speed/scale settings of all the BouncingObjects.
	/// </summary>
	public void UnifyObjectSettings ()
	{
		BasicTrialManager.BouncingObjectSettings settings = BounceObjectList [0].GetObjectSettings ();
		foreach (BounceObjectUI ui in BounceObjectList)
		{
			ui.SetSpeedScale (settings.Speed, settings.Scale);
		}
	}

	/// <summary>
	/// Returns a list of BouncingObjectSettings representing the different BouncingObjects to be included
	/// in this test.
	/// </summary>
	/// <returns>A List of BouncingObjectSettings</returns>
	public List <BasicTrialManager.BouncingObjectSettings> GetBouncingObjectSettings ()
	{
		List<BasicTrialManager.BouncingObjectSettings> settings = new List<BasicTrialManager.BouncingObjectSettings> ();

		foreach (BounceObjectUI ui in BounceObjectList)
		{
			settings.Add (ui.GetObjectSettings ());
		}

		return settings;
	}

	/// <summary>
	/// Returns a list of UnusualObjectSettings representing the different UnsusualObjects to be included
	/// in this test.
	/// </summary>
	/// <returns>A List of UnusualObjectSettings.</returns>
	public List<BasicTrialManager.UnusualObjectSettings> GetUnusualObjectSettings ()
	{
		List<BasicTrialManager.UnusualObjectSettings> settings = new List<BasicTrialManager.UnusualObjectSettings> ();

		foreach (UnusualObjectUI ui in UnusualObjectList)
		{
			settings.Add (ui.GetSettings ());
		}

		return settings;
	}

	/// <summary>
	/// Validates the UI.
	/// </summary>
	/// <param name="ErrorMessage">Contains error information on validation failure.</param>
	/// <returns>True on successful validation, false otherwise.</returns>
	public bool Validate (out string ErrorMessage)
	{
		bool bRetVal = true, bObjectTracked = false;
		if (UnusualObjectList.Count == 0)
		{
			ErrorMessage = "There is no unusual object set for the highlighted trial!";
			Highlight (true);
			bRetVal = false;
		}
		else if (BounceObjectList.Count == 0)
		{
			ErrorMessage = "There are no bouncing objects set for the highlighted trial!";
			Highlight (true);
			bRetVal = false;
		}
		else
		{
			foreach (BounceObjectUI ui in BounceObjectList)
			{
				if (ui.Tracked.isOn)
				{
					bObjectTracked = true;
				}
			}
			if (!bObjectTracked)
			{
				ErrorMessage = "None of the bouncing objects in the highlighted trial are being tracked!";
				Highlight (true);
				bRetVal = false;
			}
			else
			{
				ErrorMessage = "";
			}
		}
		return bRetVal;
	}

	/// <summary>
	/// Highlights or un-highlights the UI.
	/// </summary>
	/// <param name="isHighlighted">Whether or not to highlight the UI element.</param>
	public void Highlight (bool isHighlighted)
	{
		if (isHighlighted)
		{
			Background.color = HighlightedColor;
		}
		else
		{
			Background.color = NormalColor;
		}
	}
}
