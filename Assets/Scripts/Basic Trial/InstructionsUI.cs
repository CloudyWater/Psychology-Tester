/******************************************************************************
 * File: PreTrialUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the PreTrialUI class. This shows the UI before
 * the trail giving the participant instructions which include a preview of the
 * object which they will be tracking.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsUI : MonoBehaviour
{
	private const float START_TIMEOUT = 3.0f;

	public ObjectPreview ObjectPreviewPrefab;
	public HorizontalLayoutGroup ObjectPreviewLayout;
	public Button StartButton;

	private BasicTrialManager TrialManager;
	private float StartTimer;

	/// <summary>
	/// Initializes the UI, updating the ObjectPreview to have the passed in
	/// sprite displayed in the passed in color.
	/// </summary>
	/// <param name="objectSprite">The sprite to display.</param>
	/// <param name="objectColor">The color of the sprite.</param>
	public void InitializeUI (BasicTrialManager trialManager, BasicTrialManager.BouncingObjectSettings [] objectSettings)
	{
		TrialManager = trialManager;
		foreach (BasicTrialManager.BouncingObjectSettings	settings in objectSettings)
		{
			ObjectPreview preview = GameObject.Instantiate<ObjectPreview> (ObjectPreviewPrefab);
			preview.SetImage (BouncingObject.GetObjectSprite (settings.Shape), settings.Color.GetColor ());
			preview.GetComponent<RectTransform> ().SetParent (ObjectPreviewLayout.transform, false);
		}

		StartButton.interactable = false;
		StartTimer = 0;
	}

	/// <summary>
	/// Waits until the timeout is over then enables the start button.
	/// </summary>
	public void Update ()
	{
		StartTimer += Time.deltaTime;

		if (StartTimer > START_TIMEOUT)
		{
			StartButton.interactable = true;
		}
	}

	/// <summary>
	/// Calls back to the TrialManager to begin the trial.
	/// </summary>
	public void BeginTrial ()
	{
		TrialManager.BeginTrial ();
	}
}
