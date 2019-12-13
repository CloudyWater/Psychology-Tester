/******************************************************************************
 * File: BasicTrialManager.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the BasicTrialManager class. It is responsible 
 * for controlling the flow of the test. It shows the pre-trial UI and waits for
 * user input, then spawns all the bouncing objects and begins the trial. During
 * the trial, it may or may not spawn unusual objects. Once the trial is finished,
 * it shows the post-trial questionnaire and records the results. It may run 
 * multiple trials in each battery depending on setup.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

public class BasicTrialManager : TrialManager
{
	public const string CSV_HEADER = "Trial,Condition,Bounces by Tracked Object,Unusual Object";

	/// <summary>
	/// Contains all the customizeable information about a BouncingObject.
	/// </summary>
	[System.Serializable]
	public struct BouncingObjectSettings
	{
		public float Speed, Scale;
		public int NumberToSpawn;
		public bool bTrackedObject;
		public JsonColor Color;
		public TrialObject.ObjectShapes Shape;
		public BouncingObject.ObjectPaths Path;
	}

	/// <summary>
	/// Contains all the customizable information about an UnusualObject.
	/// </summary>
	[System.Serializable]
	public struct UnusualObjectSettings
	{
		public float Speed, Scale, SpawnTime;
		public JsonColor Color;
		public UnusualObject.ObjectShapes Shape;
		public UnusualObject.ObjectPaths Path;
	}

	/// <summary>
	/// Contains all the customizable information about a Basic Trial, including arrays
	/// for BouncingObjects, UnusualObjects, and the Questionnaire.
	/// </summary>
	[System.Serializable]
	public class BasicTrialSettings : TrialData
	{
		public int NumberOfTrials, NumberOfUnusualTrials;
		public float TrialDuration;
		public JsonColor CameraBackgroundColor;
		public BouncingObjectSettings [] BouncingObjects;
		public UnusualObjectSettings [] UnusualObjects;
	}

	/// <summary>
	/// Contains the required information about a completed trial.
	/// </summary>
	public class BasicTrialResults : TrialResults
	{
		public int TrialNumber, ConditionNumber, NumberOfBounces;
		public bool IsUnusualTrial;
		public UnusualObject.ObjectShapes UnusualObjectShape;
		public QuestionAnswer [] QuestionnaireAnswers;

		/// <summary>
		/// Returns a CSV formatted string representing this results of this trial.
		/// </summary>
		/// <returns>A CSV string with the trial data.</returns>
		public override string GetCsvData (int numQuestions)
		{
			string csv = (TrialNumber + 1) + ",";
			csv += ConditionNumber + ",";
			csv += NumberOfBounces + ",";
			if (IsUnusualTrial)
			{
				csv += UnusualObjectShape.ToString () + ",";
			}
			else
			{
				csv += "0,";
			}

			string [] responses = new string [numQuestions];

			for (int i = 0; i < responses.Length; i++)
			{
				responses[i] = "";
			}

			for (int i = 0; i < QuestionnaireAnswers.Length; i++)
			{
				responses [QuestionnaireAnswers [i].QuestionId-1] = "\"" +  QuestionnaireAnswers [i].Response + "\"";
			}

			string responseString = string.Join (",", responses);

			csv += responseString;

			return csv;
		}
	}

	/// <summary>
	/// Represents Trial State information.
	/// </summary>
	private enum TrialState
	{
		InstructionsUI, PreTrialUI, Trial, Questionnaire
	}


	public Camera Camera;
	public Questionnaire Questionnaire;
	public InstructionsUI InstructionsUI;
	public PreTrialUI PreTrialUI;
	public BouncingObject BouncingObjectPrefab;
	public UnusualObject UnusualObjectPrefab;

	private BasicTrialSettings Settings;
	private List<BouncingObject> BouncingObjects;
	private List<UnusualObject> UnusualObjects;
	private List<TrialResults> Results;
	private TrialState State;
	private bool [] UnusualTrials;
	private int TrialIteration;
	private float TrialTimer;

	/// <summary>
	/// Initializes the BasicTrialManager with the passed in data. The passed in data should be a serialized
	/// BasicTrialSettings in JSON format, which is used to initialize trial settings.
	/// </summary>
	/// <param name="trialSettings">A JSON string containing a BasicTrialSettings struct.</param>
	public override void InitializeTrials (TrialData trialSettings)
	{
		Settings = (BasicTrialSettings) trialSettings;

		//Displays the tracked object on the pre-trial UI, and shows UI.
		InstructionsUI.InitializeUI (this, Settings.BouncingObjects);
		InstructionsUI.gameObject.SetActive (true);
		State = TrialState.InstructionsUI;

		Camera.backgroundColor = Settings.CameraBackgroundColor.GetColor ();
		TrialIteration = 0;
		BouncingObjects = new List<BouncingObject> ();
		UnusualObjects = new List<UnusualObject> ();
		Questionnaire.SetQuestions (Settings.Questionnaire);
		
		//Set up array used to determine whether the current trial is an unusual one.
		UnusualTrials = new bool [Settings.NumberOfTrials];
		int numUnusual = 0, random;
		while (numUnusual < Settings.NumberOfUnusualTrials)
		{
			random = Random.Range (0, UnusualTrials.Length);
			if (UnusualTrials [random] == false)
			{
				UnusualTrials [random] = true;
				numUnusual++;
			}
		}

		Results = new List<TrialResults> ();
	}

	/// <summary>
	/// Shows the pre-trial UI displaying the object to track before the trial begins.
	/// </summary>
	public void ShowPreTrialUI ()
	{
		State = TrialState.PreTrialUI;
		foreach (BouncingObjectSettings objectSettings in Settings.BouncingObjects)
		{
			if (objectSettings.bTrackedObject)
			{
				PreTrialUI.SetUI (objectSettings);
				break;
			}
		}
	}

	/// <summary>
	/// Starts the trial. Spawns all the BouncingObjects, and starts Co-routine to
	/// spawn the UnusualObjects on the correct timer.
	/// </summary>
	public void BeginTrial ()
	{
		State = TrialState.Trial;
		TrialTimer = 0;
		BouncingObject.ResetBounces ();

		//Spawns BouncingObjects
		foreach (BouncingObjectSettings settings in Settings.BouncingObjects)
		{
			for (int i = 0; i < settings.NumberToSpawn; i++)
			{
				BouncingObject bouncingObject = GameObject.Instantiate<BouncingObject> (BouncingObjectPrefab);
				bouncingObject.SetObjectSettings (settings, Camera);
				BouncingObjects.Add (bouncingObject);
			}
		}

		//Starts Co-routine to spawn UnusualObject if this trial is an unusual one.
		if (UnusualTrials [TrialIteration])
		{
			foreach (UnusualObjectSettings settings in Settings.UnusualObjects)
			{
				StartCoroutine (SpawnUnusualObject (settings));
			}
		}
	}

	/// <summary>
	/// Spawns an unusual object based on the passed in UnusualObjectSettings.
	/// </summary>
	/// <param name="settings">An UnusualObjectSettings describing the object to spawn.</param>
	/// <returns></returns>
	IEnumerator SpawnUnusualObject (UnusualObjectSettings settings)
	{
		yield return new WaitForSeconds (settings.SpawnTime);

		UnusualObject unusualObject = GameObject.Instantiate<UnusualObject> (UnusualObjectPrefab);
		unusualObject.SetObjectSettings (settings, Camera);
		UnusualObjects.Add (unusualObject);
	}

	/// <summary>
	/// During the trial, waits for the specified amount of time then ends the trial
	/// and cleans up trial objects and object lists.
	/// </summary>
	void Update ()
	{
		if (State == TrialState.Trial)
		{
			TrialTimer += Time.deltaTime;

			if (TrialTimer >= Settings.TrialDuration)
			{
				///Trial is over!
				State = TrialState.Questionnaire;
				Questionnaire.ShowQuestionnaire ();

				foreach (UnusualObject unusual in UnusualObjects)
				{
					Destroy (unusual.gameObject);
				}
				UnusualObjects.Clear ();
				foreach (BouncingObject bouncing in BouncingObjects)
				{
					Destroy (bouncing.gameObject);
				}
				BouncingObjects.Clear ();
			}
		}
	}

	/// <summary>
	/// Starts another iteration of the trial on successful completion of the Questionnaire.
	/// TODO: Update with stat collection!!
	/// </summary>
	public override void QuestionniareFinished ()
	{
		BasicTrialResults result = new BasicTrialResults
		{
			TrialNumber = TrialIteration,
			ConditionNumber = Settings.ConditionNumber,
			IsUnusualTrial = UnusualTrials [TrialIteration],
			NumberOfBounces = BouncingObject.GetNumberOfBounces (),
			QuestionnaireAnswers = Questionnaire.GetAnswers (),
			UnusualObjectShape = Settings.UnusualObjects [0].Shape
		};
		Results.Add (result);
		TrialIteration++;
		if (TrialIteration < Settings.NumberOfTrials)
		{
			ShowPreTrialUI ();
		}
		else
		{
			TrialsFinished ();
		}
	}

	/// <summary>
	/// Returns control back to the ExperimentManager to run the next trial battery or go back to the design menu.
	/// </summary>
	public void TrialsFinished ()
	{
		ExperimentController.Instance.TrialFinished (GetTrialResults ());
	}

	/// <summary>
	/// Returns the results of the trial as a comma seperated string.
	/// </summary>
	/// <returns>Trial results data as a CSV.</returns>
	public override List<TrialResults> GetTrialResults ()
	{
		return Results;
	}
}
