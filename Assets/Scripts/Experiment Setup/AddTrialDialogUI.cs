/******************************************************************************
 * File: AddTrialWindowUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the AddTrialWindowUI class. It shows the dialog
 * for selecting the type of test to add to the trial setup.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddTrialDialogUI : MonoBehaviour
{
	public ExperimentDesignUI ExperimentDesignUI;
	public Button AddTrialButton;

	public Dropdown TrialTypeDropdown;

	/// <summary>
	/// Sets up the AddTrialButton to add a trial to the ExperimentDesignUI.
	/// </summary>
	void Start ()
	{
		AddTrialButton.onClick.AddListener (delegate { ExperimentDesignUI.AddTrialUI ((TrialManager.TrialType) TrialTypeDropdown.value); });
	}
}
