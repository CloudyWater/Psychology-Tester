/******************************************************************************
 * File: Questionnaire.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the Questionnaire class, as well as a QuestionType
 * enum and a Question struct. It handles displaying the questionnaire after
 * the trial is over as well as validating and collecting responses.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Questionnaire : MonoBehaviour
{
	public const string YES = "Yes";
	public const string NO = "No";

	/// <summary>
	/// Defines the types of available questions.
	/// </summary>
	public enum QuestionType
	{
		YesNo, Response, Scale
	}

	/// <summary>
	/// Defines question data.
	/// </summary>
	[System.Serializable]
	public struct Question
	{
		public int QuestionId;
		public QuestionType QuestionType;
		public bool AnswerRequired;
		public string QuestionText;
	}

	public TrialManager TrialManager;
	public QuestionUI QuestionPrefab;
	public VerticalLayoutGroup QuestionDisplay;

	private List<QuestionUI> DisplayedQuestions;

	/// <summary>
	/// Sets the Questions to be displayed in this questionnaire to the passed in
	/// array of Questions.
	/// </summary>
	/// <param name="questions">An array of Questions to be displayed.</param>
	public void SetQuestions (Question[] questions)
	{
		if (DisplayedQuestions == null)
		{
			DisplayedQuestions = new List<QuestionUI> ();
		}
		else
		{
			foreach (QuestionUI questionUI in DisplayedQuestions)
			{
				Destroy (questionUI);
			}
			DisplayedQuestions.Clear ();
		}

		foreach (Question question in questions)
		{
			QuestionUI newUI = Instantiate<QuestionUI> (QuestionPrefab);
			newUI.SetQuestion (question);
			newUI.GetComponent<RectTransform> ().SetParent (QuestionDisplay.transform, false);
			DisplayedQuestions.Add (newUI);
		}
	}

	/// <summary>
	/// Displays the questionnaire. Clears out previous answers.
	/// </summary>
	public void ShowQuestionnaire ()
	{
		gameObject.SetActive (true);
		foreach (QuestionUI ui in DisplayedQuestions)
		{
			ui.Clear ();
		}
	}

	/// <summary>
	/// Hides the questionnaire.
	/// </summary>
	public void HideQuestionnaire ()
	{
		gameObject.SetActive (false);
	}

	public void QuestionnaireFinished ()
	{
		bool bValidQuestionnaire = true;
		string validationErrors = "";

		foreach (QuestionUI questionUI in DisplayedQuestions)
		{
			if (!questionUI.Validate (out validationErrors))
			{
				bValidQuestionnaire = false;
				questionUI.Highlight (true);
				break;
			}
		}

		if (bValidQuestionnaire)
		{
			HideQuestionnaire ();
			TrialManager.QuestionniareFinished ();
		}
		else
		{
			DialogManager.Instance.MessageDialog.DisplayMessage ("Unanswered Question!", validationErrors);
		}
	}

	/// <summary>
	/// Returns a string array containing the answers for each of the questions shown.
	/// </summary>
	/// <returns>An array of string responses.</returns>
	public TrialManager.QuestionAnswer[] GetAnswers ()
	{
		List<TrialManager.QuestionAnswer> Answers = new List<TrialManager.QuestionAnswer> ();

		foreach (QuestionUI question in DisplayedQuestions)
		{
			TrialManager.QuestionAnswer response = new TrialManager.QuestionAnswer
			{
				QuestionId = question.GetQuestionId (),
				Response = question.GetResponse ()
			};
			Answers.Add (response);
		}

		return Answers.ToArray ();
	}
}
