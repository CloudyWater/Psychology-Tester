/******************************************************************************
 * File: BasicTrialDesignUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the BasicTrialDesignUI, which subclasses 
 * TrialDesignUI. This UI element is used to design a battery of the basic 
 * bouncing object trials. It has UI elements for the number of trials/unusual
 * trials, the length of the trial, and the color of the background. It also 
 * has a BasicTrialObjectDesignUI and a QuestionnaireDesignUI.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

[RequireComponent (typeof (Image))]
public class BasicTrailDesignUI : TrialDesignUI, IValidatedUI
{
	public ValidatedInputField NumberOfTrials, NumberUnusualTrials, TrialDuration;
	public Image CameraBackground;
	public ColorPickerUI CameraBackgroundPicker;

	public BasicTrialObjectDesignUI TrialObjectsUI;
	public QuestionnaireDesignUI QuestionnaireUI;

	private Image Background;

	/// <summary>
	/// Updates the test camera background color display.
	/// </summary>
	public void OnBackgroundColorChanged ()
	{
		CameraBackground.color = CameraBackgroundPicker.GetJSONColor ().GetColor ();
	}

	/// <summary>
	/// Validates each of the basic trial elements, then requires validation from
	/// the TrialObjectsUI and QuestionnaireUI. Highlights offending elements, then
	/// returns a descriptive error through ErrorString.
	/// </summary>
	/// <param name="ErrorString">Contains validation error messages.</param>
	/// <returns>True if validation succeeds, false otherwise.</returns>
	public override bool Validate (out string ErrorString)
	{
		//Test each UI element, failing out on first failure.
		bool isValid = NumberOfTrials.Validate (out ErrorString);

		if (isValid)
		{
			isValid = NumberUnusualTrials.Validate (out ErrorString);
		}
		if (isValid)
		{
			isValid = TrialDuration.Validate (out ErrorString);
		}
		if (isValid)
		{
			isValid = TrialObjectsUI.Validate (out ErrorString);
		}
		if (isValid)
		{
			isValid = QuestionnaireUI.Validate (out ErrorString);
		}

		return isValid;
	}

	/// <summary>
	/// Returns the BasicTrialSettings from GetTrialSettings as a JSON formatted string.
	/// </summary>
	/// <returns>A JSON formatted string of the desinged bouncing object trial.</returns>
	public override TrialManager.TrialData GetTrialData ()
	{
		return GetTrialSettings ();
	}

	/// <summary>
	/// Creates a new BasicTrialSettings from the UI as designed.
	/// </summary>
	/// <returns>A BasicTrialSettings containing the trial settings.</returns>
	private BasicTrialManager.BasicTrialSettings GetTrialSettings ()
	{

		BasicTrialManager.BasicTrialSettings settings = new BasicTrialManager.BasicTrialSettings
		{
			NumberOfTrials = int.Parse (NumberOfTrials.text),
			NumberOfUnusualTrials = int.Parse (NumberUnusualTrials.text),
			TrialDuration = float.Parse (TrialDuration.text),
			BouncingObjects = TrialObjectsUI.GetBouncingObjectSettings ().ToArray (),
			UnusualObjects = TrialObjectsUI.GetUnusualObjectSettings ().ToArray (),
			Questionnaire = QuestionnaireUI.GetQuestions ().ToArray (),
			CameraBackgroundColor = CameraBackgroundPicker.GetJSONColor ()
		};

		return settings;
	}

	/// <summary>
	/// Sets the UI to display the passed in BasicTrialSettings.
	/// </summary>
	/// <param name="trialSettings">A BasicTrialSettings as a JSON string.</param>
	public override void SetTrialData (TrialManager.TrialData trialSettings)
	{
		BasicTrialManager.BasicTrialSettings settings = (BasicTrialManager.BasicTrialSettings) (trialSettings);

		NumberOfTrials.text = settings.NumberOfTrials.ToString ();
		NumberUnusualTrials.text = settings.NumberOfUnusualTrials.ToString ();
		TrialDuration.text = settings.TrialDuration.ToString ();
		CameraBackgroundPicker.SetJSONColor (settings.CameraBackgroundColor);
		CameraBackground.color = settings.CameraBackgroundColor.GetColor ();

		foreach (BasicTrialManager.BouncingObjectSettings bouncingObject in settings.BouncingObjects)
		{
			TrialObjectsUI.AddTrialObject (bouncingObject);
		}

		foreach (BasicTrialManager.UnusualObjectSettings unusualObject in settings.UnusualObjects)
		{
			TrialObjectsUI.AddTrialObject (unusualObject);
		}

		foreach (Questionnaire.Question question in settings.Questionnaire)
		{
			QuestionnaireUI.AddQuestion (question);
		}
	}

	/// <summary>
	/// Duplicates the UI element.
	/// </summary>
	public void DuplicateUI ()
	{
		ExperimentDesignUI designUI = FindObjectOfType<ExperimentDesignUI> ();
		if (designUI != null)
		{
			designUI.AddTrialUI (TrialType, GetTrialData ());
		}
	}
}
