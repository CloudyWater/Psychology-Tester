/******************************************************************************
 * File: ExperimentDesignUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the ExperimentDesignUI class. This class handles
 * managing the displayed TrialDesignUIs and interfacing with the 
 * ExperimentController to manage starting/stopping/saving tests.
 * ***************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

public class ExperimentDesignUI : MonoBehaviour, IValidatedUI
{
	public GameObject [] TrialUIPrefabs;

	public VerticalLayoutGroup TrialDisplayGroup;

	public Button SaveButton, StartButton;

	public ValidatedInputField ParticipantID;

	public Text ExperimentName;

	private ExperimentController.ExperimentSave OldExperimentState;

	/// <summary>
	/// Sets up save button and start button listeners.
	/// </summary>
	void Start ()
	{
		SaveButton.onClick.AddListener (delegate { ValidateAndSave (); });
		StartButton.onClick.AddListener (delegate { ValidateAndStart (); });
	}

	/// <summary>
	/// Sets the ParticipantID text. If the ID is a number, increments it.
	/// </summary>
	/// <param name="participantId">The participant ID</param>
	public void SetParticipantId (string participantId)
	{
		int participantNumber;

		if (int.TryParse (participantId, out participantNumber))
		{
			participantNumber++;
			ParticipantID.text = participantNumber.ToString ();
		}
		else
		{
			ParticipantID.text = participantId;
		}
	}

	/// <summary>
	/// Sets the current experiment.
	/// </summary>
	/// <param name="experiment">The experiment.</param>
	/// <param name="experimentName">The name of the experiment.</param>
	public void SetExperiment (ExperimentController.ExperimentSave experiment)
	{
		OldExperimentState = experiment;
		foreach (ExperimentController.TrialSave trial in OldExperimentState.ExperimentTrials)
		{
			AddTrialUI (trial.TrialType, trial.TrialData);
		}
	}

	/// <summary>
	/// Sets the current name of the experiment.
	/// </summary>
	/// <param name="experimentName">The name of the experiment.</param>
	public void SetExperimentName (string experimentName)
	{
		ExperimentName.text = experimentName;
	}

	/// <summary>
	/// Adds a TrialDesignUI of the specified TrialType to the display.
	/// </summary>
	/// <param name="trialType">The type of TrialDesignUI to add.</param>
	public void AddTrialUI (TrialManager.TrialType trialType)
	{
		GameObject trialUIObject = GameObject.Instantiate (TrialUIPrefabs [(int) trialType]);
		trialUIObject.GetComponent<RectTransform> ().SetParent (TrialDisplayGroup.transform, false);
		TrialDesignUI designUI = trialUIObject.GetComponent<TrialDesignUI> ();
		designUI.SetParentUI (this);
	}

	/// <summary>
	/// Adds a TrialDesignUi of the specified TrialType and fills it with the passed in
	/// trialData.
	/// </summary>
	/// <param name="trialType">The type of trial to add.</param>
	/// <param name="trialData">The settings data for the trial.</param>
	public void AddTrialUI (TrialManager.TrialType trialType, TrialManager.TrialData trialData)
	{
		GameObject trialUIObject = GameObject.Instantiate (TrialUIPrefabs [(int) trialType]);
		trialUIObject.GetComponent<RectTransform> ().SetParent (TrialDisplayGroup.transform, false);
		TrialDesignUI designUI = trialUIObject.GetComponent<TrialDesignUI> ();
		designUI.SetParentUI (this);
		designUI.SetTrialData (trialData);
	}

	/// <summary>
	/// Removes a TrialDesignUI from the listing.
	/// </summary>
	/// <param name="trialUI">The UI to remove.</param>
	public void RemoveTrialUI (TrialDesignUI trialUI)
	{
		Destroy (trialUI.gameObject);
	}

	/// <summary>
	/// Removes all TrialDesignUI from the listing.
	/// </summary>
	public void ClearUI ()
	{
		foreach (Transform ui in TrialDisplayGroup.transform)
		{
			Destroy (ui.gameObject);
		}
	}

	/// <summary>
	/// Validates the current test setup before displaying the save dialog.
	/// </summary>
	public void ValidateAndSave ()
	{
		string errorMessage;
		if (Validate (out errorMessage))
		{
			ExperimentController.Instance.SaveExperiment ();
		}
		else
		{
			DialogManager.Instance.MessageDialog.DisplayMessage ("Validation Error!", errorMessage);
		}
	}

	/// <summary>
	/// Validates the current test setup before starting the experiment.
	/// </summary>
	public void ValidateAndStart ()
	{
		string errorMessage;
		if (Validate (out errorMessage))
		{
			string oldStateString = Newtonsoft.Json.JsonConvert.SerializeObject (OldExperimentState);
			string newStateString = Newtonsoft.Json.JsonConvert.SerializeObject (GetExperimentData ());
			if (ExperimentName.text == "")
			{
				DialogManager.Instance.MessageDialog.DisplayMessage ("Test Setup Not Saved!", "The test setup has not been saved. Please save the test setup to start experimenting.");
			}
			else if (oldStateString != newStateString)
			{
				DialogManager.Instance.ConfirmationDialog.DisplayConfirmation ("Test Setup Changed!", "The test setup has changed. Save changes to test setup?",
					delegate { ExperimentController.Instance.SaveExperiment (); });
			}
			else if (ParticipantID.Validate (out errorMessage))
			{
				if (ExperimentController.Instance.DoResultsExist (ParticipantID.text))
				{
					DialogManager.Instance.ConfirmationDialog.DisplayConfirmation ("Duplicate Participant ID", "A data record for this experiment/participant ID record already exists. Continue and overwrite?",
						delegate { ExperimentController.Instance.StartExperiment (ExperimentName.text, ParticipantID.text); });
				}
				else
				{
					ExperimentController.Instance.StartExperiment (ExperimentName.text, ParticipantID.text);
				}
			}
			else
			{
				DialogManager.Instance.MessageDialog.DisplayMessage ("Missing Participant ID", "Please set a participant ID. A participant ID is needed to save collected experiment data.");
			}
		}
		else
		{
			DialogManager.Instance.MessageDialog.DisplayMessage ("Setup Validation Error!", errorMessage);
		}
	}

	/// <summary>
	/// Validates the currently displayed experiment.
	/// </summary>
	/// <param name="ErrorMessage">Contains detailed error information.</param>
	/// <returns>True if validated, false otherwise.</returns>
	public bool Validate (out string ErrorMessage)
	{
		bool bIsValid = true;
		ErrorMessage = "";

		foreach (Transform ui in TrialDisplayGroup.transform)
		{
			TrialDesignUI designUI = ui.GetComponent<TrialDesignUI> ();
			if (!designUI.Validate (out ErrorMessage))
			{
				bIsValid = false;
				break;
			}
		}

		return bIsValid;
	}

	/// <summary>
	/// Returns the ExperimentData displayed in the design ui.
	/// </summary>
	/// <returns>an ExperimentData struct containing the displayed UI.</returns>
	public ExperimentController.ExperimentSave GetExperimentData ()
	{
		int questionId = 1;

		ExperimentController.ExperimentSave experimentData = new ExperimentController.ExperimentSave ();

		ExperimentController.TrialSave [] experimentTrials = new ExperimentController.TrialSave [TrialDisplayGroup.transform.childCount];

		for (int i = 0; i < experimentTrials.Length; i++)
		{
			TrialDesignUI designUI = TrialDisplayGroup.transform.GetChild (i).GetComponent<TrialDesignUI> ();
			experimentTrials [i] = new ExperimentController.TrialSave
			{
				TrialType = designUI.TrialType,
				TrialData = designUI.GetTrialData ()
			};

			experimentTrials [i].TrialData.ConditionNumber = i;

			for (int j = 0; j < experimentTrials [i].TrialData.Questionnaire.Length; j++)
			{
				experimentTrials [i].TrialData.Questionnaire [j].QuestionId = questionId;
				questionId++;
			}
		}

		experimentData.ExperimentTrials = experimentTrials;

		return experimentData;
	}

	/// <summary>
	/// Loads an experiment setup.
	/// </summary>
	public void LoadExperiment ()
	{
		ExperimentController.Instance.LoadExperiment ();
	}

	/// <summary>
	/// Quits the application.
	/// </summary>
	public void Quit ()
	{
		Application.Quit ();
	}
}
