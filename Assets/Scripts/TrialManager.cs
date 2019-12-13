/******************************************************************************
 * File: TrialManager.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the abstract TrialManager class. It holds methods
 * common to all TrialManager type objects.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudyWaterGames.Interfaces;

public abstract class TrialManager : MonoBehaviour
{

	/// <summary>
	/// Defines the different types of trials
	/// </summary>
	public enum TrialType
	{
		BasicTrial
	}

	/// <summary>
	/// Basic TrialData class. Holds members and methods common to all
	/// TrialData classes.
	/// </summary>
	public class TrialData
	{
		public int ConditionNumber;
		public Questionnaire.Question [] Questionnaire;
	}


	/// <summary>
	/// Holds basic information about a questionnaire response.
	/// </summary>
	public struct QuestionAnswer
	{
		public int QuestionId;
		public string Response;
	}

	/// <summary>
	/// Abstract base TrialResults class. Implements ITrialResults interface.
	/// </summary>
	public abstract class TrialResults : ITrialResults
	{
		public abstract string GetCsvData (int numQuestions);
	}

	/// <summary>
	/// Defines a JSON-serializeable color.
	/// </summary>
	[System.Serializable]
	public struct JsonColor
	{
		public int R, G, B;

		/// <summary>
		/// Returns a Color representation of this JSONColor.
		/// </summary>
		/// <returns>A Color.</returns>
		public Color GetColor ()
		{
			return new Color (Map (0, 1, 0, 255, R), Map (0, 1, 0, 255, G), Map (0, 1, 0, 255, B));
		}

		/// <summary>
		/// Float map function.
		/// </summary>
		/// <param name="newFrom">new map min</param>
		/// <param name="newTo">new map max</param>
		/// <param name="oldFrom">old map min</param>
		/// <param name="oldTo">old map max</param>
		/// <param name="value">value to map</param>
		/// <returns>value mapped from old range to new range.</returns>
		public static float Map (float newFrom, float newTo, float oldFrom, float oldTo, float value)
		{
			if (value <= oldFrom)
			{
				return newFrom;
			}
			else if (value >= oldTo)
			{
				return newTo;
			}
			else
			{
				return (newTo - newFrom) * ((value - oldFrom) / (oldTo - oldFrom)) + newFrom;
			}
		}
	}

	private List<TrialResults> Results;

	/// <summary>
	/// Returns the header line for a CSV file deliniating the fields contained in the data.
	/// </summary>
	/// <param name="type">The type of trial.</param>
	/// <param name="trialData">The trial's TrialData.</param>
	/// <param name="numQuestions">The number of questions in questionnaires, returned as an out variable.</param>
	/// <returns>A string containing the CSV header for that Trial.</returns>
	public static string GetCsvHeader (TrialType type, TrialData [] trialData, out int numQuestions)
	{
		string header = "";
		numQuestions = 0;

		switch (type)
		{
			case TrialType.BasicTrial:
				header = BasicTrialManager.CSV_HEADER;
				for (int i = 0; i < trialData.Length; i++)
				{
					numQuestions += trialData [i].Questionnaire.Length;
					for (int j = 0; j < trialData[i].Questionnaire.Length; j++)
					{
						header += "," + "\"" + trialData [i].Questionnaire [j].QuestionText + "\"";
					}
				}
				break;
		}

		header += System.Environment.NewLine;

		return header;
	}

	/// <summary>
	/// Initializes the trial with the passed in trial data.
	/// </summary>
	/// <param name="trialData">A JSON formatted string containing trial data.</param>
	public abstract void InitializeTrials (TrialData trialData);
	
	/// <summary>
	/// Called when the questionnaire at the end of the trial is finished.
	/// </summary>
	public abstract void QuestionniareFinished ();

	/// <summary>
	/// Returns the results of the trial in a string.
	/// </summary>
	/// <returns>The results of the trial as a string.</returns>
	public abstract List<TrialResults> GetTrialResults ();
}
