/******************************************************************************
 * File: TrialDesignUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the abstract TrialDesignUI class. It contains
 * a set of methods that each TrialDesignUI subclass should implement in order
 * to be used in a generic fashion by the ExperimentDesignUI. In particular,
 * GetTrialData and SetTrialData to save/load test settings from the UI.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

[RequireComponent (typeof (Image))]
public abstract class TrialDesignUI : MonoBehaviour, IValidatedUI
{
	public TrialManager.TrialType TrialType;
	public Button RemoveTrialButton;

	private ExperimentDesignUI ExperimentDesignUI;

	/// <summary>
	/// Sets the ExperimentDesignUI, updates the RemoveTrialButton to remove the trial.
	/// </summary>
	/// <param name="parentUI">The ExperimentDesignUI holding this TrialDesignUI.</param>
	public void SetParentUI (ExperimentDesignUI parentUI)
	{
		ExperimentDesignUI = parentUI;
		RemoveTrialButton.onClick.AddListener (delegate { ExperimentDesignUI.RemoveTrialUI (this); });
	}

	/// <summary>
	/// Implementations of this method should validate all required UI elements, find and highlight
	/// any invalid UI elements. Returns true if no validation errors, false if not.
	/// </summary>
	/// <param name="ErrorString">Contains error information if a validation error occurs.</param>
	/// <returns>True if validated, false otherwise.</returns>
	public abstract bool Validate (out string ErrorString);

	/// <summary>
	/// Implementations of this method should convert all necessary trial data to JSON format
	/// and return them as a string value.
	/// </summary>
	/// <returns>A JSON string containing all information about the trial as designed.</returns>
	public abstract TrialManager.TrialData GetTrialData ();

	/// <summary>
	/// Implementations of this method should take the passed in trial data and inflate the UI
	/// elements as necessary to display the trial information.
	/// </summary>
	/// <param name="trialData">A JSON string containing the trial data.</param>
	public abstract void SetTrialData (TrialManager.TrialData trialData);

	/// <summary>
	/// Moves the UI up in the display and testing order.
	/// </summary>
	public void MoveUp ()
	{
		if (transform.GetSiblingIndex () > 0)
		{
			transform.SetSiblingIndex (transform.GetSiblingIndex () - 1);
		}
	}

	/// <summary>
	/// Moves the UI down in the display and testing order.
	/// </summary>
	public void MoveDown ()
	{
		if (transform.GetSiblingIndex () < transform.parent.childCount)
		{
			transform.SetSiblingIndex (transform.GetSiblingIndex () + 1);
		}
	}

	/// <summary>
	/// Deletes the design ui.
	/// </summary>
	public void DeleteUI ()
	{
		Destroy (this.gameObject);
	}
}
