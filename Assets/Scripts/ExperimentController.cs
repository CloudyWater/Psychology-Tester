/******************************************************************************
 * File: ExperimentController.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the ExperimentController class, as well as some
 * data structures for storing and loading experiment setups. This class is
 * responsible for saving and loading experiment setups and running experiments.
 * ***************************************************************************/
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentController : MonoBehaviour
{
	public const string SAVED_TESTS = "/Tests";
	public const string TEST_EXTENTION = "exp";
	public const string SAVED_RESULTS = "/Results";
	public const string CSV_EXTENTION = "csv";

	private const int SCENE_DESIGN_INDEX = 0;
	private const int SCENEE_FINISHED_INDEX= 2;

	/// <summary>
	/// Unity singleton pattern for easy access of ExperimentController methods.
	/// </summary>

	private static ExperimentController instance;

	public static ExperimentController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<ExperimentController> ();
				if (instance == null)
				{
					GameObject instanceObj = new GameObject ("ExperimentController", typeof (ExperimentController));
					instance = instanceObj.GetComponent<ExperimentController> ();
				}
			}
			return instance;
		}
	}

	private ExperimentController () { }

	/// <summary>
	/// Generic contianer for trial settings data. Contains a type identifier
	/// and a JSON formatted string representing the trial settings.
	/// </summary>
	public struct TrialSave
	{
		public TrialManager.TrialType TrialType;
		public TrialManager.TrialData TrialData;
	}

	/// <summary>
	/// Wrapper for an array of TrialSaves for correct JSONUtility serialization and test sequencing.
	/// </summary>
	public struct ExperimentSave
	{
		public TrialSave [] ExperimentTrials;
	}

	private ExperimentDesignUI ExperimentDesignUI;

	private ExperimentSave CurrentExperiment;
	private int CurrentTrial;
	private string CurrentExperimentName, CurrentParticipantId;

	private List<TrialManager.TrialResults> ExperimentResults;

	private Newtonsoft.Json.JsonSerializerSettings SerializerSettings;

	/// <summary>
	/// Called once on the first frame. Sets up the saved tests directory if it doesn't exist,
	/// then calls DontDestroyOnLoad() to persist through scene changes. Registers OnTrialLoaded
	/// to the scene loaded event.
	/// </summary>
	public void Awake ()
	{
		if (!Directory.Exists (Application.dataPath + SAVED_TESTS))
		{
			Directory.CreateDirectory (Application.dataPath + SAVED_TESTS);
		}

		if (!Directory.Exists (Application.dataPath + SAVED_RESULTS))
		{
			Directory.CreateDirectory (Application.dataPath + SAVED_RESULTS);
		}

		ExperimentDesignUI = FindObjectOfType<ExperimentDesignUI> ();

		SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings ();
		SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;

		DontDestroyOnLoad (this);
		SceneManager.sceneLoaded += OnSceneLoaded;
		CurrentTrial = 0;
		ExperimentResults = new List<TrialManager.TrialResults> ();
	}

	/// <summary>
	/// Collects the ExperimentData from the ExperimentDesignUI, then writes them to a file.
	/// </summary>
	public void SaveExperiment ()
	{
		CurrentExperiment = ExperimentDesignUI.GetExperimentData ();
		string experimentJSON = Newtonsoft.Json.JsonConvert.SerializeObject (CurrentExperiment, SerializerSettings);
		string filePath = SFB.StandaloneFileBrowser.SaveFilePanel ("Save Experiment", Application.dataPath + SAVED_TESTS, CurrentExperimentName, TEST_EXTENTION);

		if (filePath != "")
		{
			File.WriteAllText (filePath, experimentJSON);

			CurrentExperimentName = GetNameFromPath (filePath);
			ExperimentDesignUI.ClearUI ();
			ExperimentDesignUI.SetExperimentName (CurrentExperimentName);
			ExperimentDesignUI.SetExperiment (CurrentExperiment);
		}
	}

	/// <summary>
	/// Displays an OpenFilePanel, then loads the selected experiment file.
	/// </summary>
	public void LoadExperiment ()
	{
		string[] files = SFB.StandaloneFileBrowser.OpenFilePanel ("Load Experiment", Application.dataPath + SAVED_TESTS, TEST_EXTENTION, false);
		if(files != null && files.Length > 0 && files[0] != null)
		{
			CurrentExperimentName = GetNameFromPath (files [0]);
			string fileData = File.ReadAllText (files[0]);
			ExperimentSave experiment = Newtonsoft.Json.JsonConvert.DeserializeObject<ExperimentSave> (fileData, SerializerSettings);
			ExperimentDesignUI.ClearUI ();
			ExperimentDesignUI.SetExperiment (experiment);
			ExperimentDesignUI.SetExperimentName (CurrentExperimentName);
		}
	}

	/// <summary>
	/// Returns the file name from the given path.
	/// </summary>
	/// <param name="filePath">The file's path.</param>
	/// <returns>The file name without extention or preceding path info.</returns>
	private string GetNameFromPath (string filePath)
	{
		string name = "";
		string [] fileParts = filePath.Split ('/', '\\');
		foreach (string part in fileParts)
		{
			if (part.EndsWith (ExperimentController.TEST_EXTENTION))
			{
				name = part.Remove (part.IndexOf ('.'));
			}
		}

		return name;
	}

	/// <summary>
	/// Sets the Current Experiment data, then loads the first trial scene.
	/// </summary>
	/// <param name="experimentName">The name of the current experiment setup (for saving results)</param>
	/// <param name="participantId">The current participant ID (for saving results)</param>
	public void StartExperiment (string experimentName, string participantId)
	{
		CurrentExperimentName = experimentName;
		CurrentParticipantId = participantId;
		CurrentExperiment = ExperimentDesignUI.GetExperimentData ();
		ExperimentResults.Clear ();
		LoadTrial (CurrentExperiment.ExperimentTrials [CurrentTrial]);

	}

	/// <summary>
	/// Loads the trial scene based on the passed in TrialData.
	/// </summary>
	/// <param name="trialData">The TrialData to load the scene from.</param>
	private void LoadTrial (TrialSave trialData)
	{
		SceneManager.LoadScene ((int) trialData.TrialType + 1);
	}

	/// <summary>
	/// Called when a scene is loaded. Currently, looks for a TrialManager and passes it
	/// the TrialJSON for initialization. If no TrialManager is found, assumes is in menu
	/// and rebuilds UI for current trial setup.
	/// </summary>
	/// <param name="trialScene"></param>
	/// <param name="loadMode"></param>
	private void OnSceneLoaded (Scene trialScene, LoadSceneMode loadMode)
	{
		TrialManager manager = FindObjectOfType<TrialManager> ();
		if (manager != null)
		{
			manager.InitializeTrials (CurrentExperiment.ExperimentTrials [CurrentTrial].TrialData);
		}
		else
		{
			ExperimentDesignUI = FindObjectOfType<ExperimentDesignUI> ();
			if (ExperimentDesignUI != null)
			{
				ExperimentDesignUI.SetExperiment (CurrentExperiment);
				ExperimentDesignUI.SetParticipantId (CurrentParticipantId);
				ExperimentDesignUI.SetExperimentName (CurrentExperimentName);
			}
		}
	}

	/// <summary>
	/// Called when the currently running trial is finished. Re-loads the experiment design scene.
	/// </summary>
	public void TrialFinished (List<TrialManager.TrialResults> TrialResults)
	{
		CurrentTrial++;

		ExperimentResults.AddRange (TrialResults);

		if (CurrentTrial < CurrentExperiment.ExperimentTrials.Length)
		{
			LoadTrial (CurrentExperiment.ExperimentTrials [CurrentTrial]);
		}
		else
		{
			ExportResults ();
			CurrentTrial = 0;
			SceneManager.LoadScene (SCENEE_FINISHED_INDEX);
		}
	}

	/// <summary>
	/// Exports all the results of the trials to CSV format.
	/// </summary>
	public void ExportResults ()
	{
		int numBasicTrialQuestions;
		List<BasicTrialManager.BasicTrialSettings> basicTrialSettings = new List<BasicTrialManager.BasicTrialSettings> ();
		List<BasicTrialManager.BasicTrialResults> basicTrialResults = new List<BasicTrialManager.BasicTrialResults> ();

		//Seperate out results by trial type.
		foreach (TrialManager.TrialResults results in ExperimentResults)
		{
			if (results is BasicTrialManager.BasicTrialResults)
			{
				basicTrialResults.Add ((BasicTrialManager.BasicTrialResults) results);
			}
		}

		//Seperate out trial conditions by trial type.
		foreach (TrialSave condition in CurrentExperiment.ExperimentTrials)
		{
			switch (condition.TrialType)
			{
				case TrialManager.TrialType.BasicTrial:
					basicTrialSettings.Add ((BasicTrialManager.BasicTrialSettings) condition.TrialData);
					break;
			}
		}

		//Export each type of trial in its own file.

		string BouncingObjectData = TrialManager.GetCsvHeader (TrialManager.TrialType.BasicTrial, basicTrialSettings.ToArray (), out numBasicTrialQuestions);

		for (int i = 0; i < basicTrialResults.Count; i++) 
		{
			BouncingObjectData += basicTrialResults[i].GetCsvData (numBasicTrialQuestions);
			if (i < basicTrialResults.Count - 1)
			{
				BouncingObjectData += "," + System.Environment.NewLine;
			}
		}

		File.WriteAllText (Application.dataPath + SAVED_RESULTS + Path.DirectorySeparatorChar + CurrentExperimentName + " participant " + CurrentParticipantId + "." + CSV_EXTENTION, BouncingObjectData);

	}

	/// <summary>
	/// Returns to the design scene after a trial.
	/// </summary>
	public void ReturnToDesign ()
	{
		SceneManager.LoadScene (SCENE_DESIGN_INDEX);
	}

	/// <summary>
	/// Returns true if there is a file with the participantId and ExperimentName already.
	/// </summary>
	/// <param name="participantId">The participantId to look for.</param>
	/// <returns></returns>
	public bool DoResultsExist (string participantId)
	{
		return File.Exists (Application.dataPath + SAVED_RESULTS + Path.DirectorySeparatorChar + CurrentExperimentName + " participant " + participantId + "." + CSV_EXTENTION);
	}
}
