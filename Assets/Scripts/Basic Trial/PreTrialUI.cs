/******************************************************************************
 * File: PreTrialUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the PreTrialUI class, which displays a UI before
 * each trial showing the object to track. It has a button to start the trial
 * which activates after a short timer.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreTrialUI : MonoBehaviour
{
	private const float ACTIVATION_TIME = 3.0f;

	public ObjectPreview Preview;
	public Button StartButton;

	private float ActivationTimer;

	/// <summary>
	/// Sets the ObjectPreview to display the tracked object for the trial.
	/// </summary>
	/// <param name="settings">The tracked object's settings.</param>
	public void SetUI (BasicTrialManager.BouncingObjectSettings settings)
	{
		Preview.SetImage (BouncingObject.GetObjectSprite (settings.Shape), settings.Color.GetColor ());
		StartButton.interactable = false;
		ActivationTimer = 0;
		gameObject.SetActive (true);
	}

	/// <summary>
	/// Waits for a set amount of time, then enables the begin trial button.
	/// </summary>
	private void Update ()
	{
		ActivationTimer += Time.deltaTime;
		if (ActivationTimer >= ACTIVATION_TIME)
		{
			StartButton.interactable = true;
		}
	}
}
