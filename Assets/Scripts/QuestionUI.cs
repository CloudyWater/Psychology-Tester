/******************************************************************************
 * File: QuestionUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contiains the QuestionUI class and some structures for
 * containing different QuestionUI types. The QuestionUI class displays a
 * question and collects the response.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

[RequireComponent (typeof (Image))]
public class QuestionUI : MonoBehaviour, IValidatedUI, IHighlightableUI
{
	/// <summary>
	/// Contians the Response question type UI elements.
	/// </summary>
	[System.Serializable]
	public struct ResponseUI
	{
		public GameObject Display;
		public InputField ResponseField;
	}

	/// <summary>
	/// Contains the Yes/No question type UI elements.
	/// </summary>
	[System.Serializable]
	public struct YesNoUI
	{
		public GameObject Display;
		public ToggleGroup ToggleGroup;
		public Toggle Yes, No;
	}

	/// <summary>
	/// Contains the scale question type UI elements.
	/// </summary>
	[System.Serializable]
	public struct ScaleUI
	{
		public GameObject Display;
		public ToggleGroup ScaleGroup;
	}

	public Text QuestionDisplay;

	public ResponseUI ResponseDisplay;
	public YesNoUI YesNoDisplay;
	public ScaleUI ScaleDisplay;
	public Color NormalColor, HighlightedColor;

	private Questionnaire.Question Question;
	private Image Background;

	/// <summary>
	/// Displays the passed in Question and sets Qestion.
	/// </summary>
	/// <param name="question">The Question to display.</param>
	public void SetQuestion (Questionnaire.Question question)
	{
		Question = question;
		QuestionDisplay.text = Question.QuestionText;
		switch (Question.QuestionType)
		{
			case Questionnaire.QuestionType.Response:
				ResponseDisplay.Display.SetActive (true);
				YesNoDisplay.Display.SetActive (false);
				ScaleDisplay.Display.SetActive (false);
				break;
			case Questionnaire.QuestionType.YesNo:
				ResponseDisplay.Display.SetActive (false);
				YesNoDisplay.Display.SetActive (true);
				ScaleDisplay.Display.SetActive (false);
				break;
			case Questionnaire.QuestionType.Scale:
				ResponseDisplay.Display.SetActive (false);
				YesNoDisplay.Display.SetActive (false);
				ScaleDisplay.Display.SetActive (false);
				break;
		}

		Background = GetComponent<Image> ();
	}

	/// <summary>
	/// Clears any responses from the UI.
	/// </summary>
	public void Clear ()
	{
		switch (Question.QuestionType)
		{
			case Questionnaire.QuestionType.Response:
				ResponseDisplay.ResponseField.text = "";
				break;
			case Questionnaire.QuestionType.YesNo:
				YesNoDisplay.Yes.isOn = false;
				YesNoDisplay.No.isOn = false;
				break;
			case Questionnaire.QuestionType.Scale:
				break;
		}
	}

	/// <summary>
	/// Validates the response field of the QuestionUI.
	/// </summary>
	/// <param name="ErrorMessage">Contains detailed error information.</param>
	/// <returns>True if validation is successful, false otherwise.</returns>
	public bool Validate (out string ErrorMessage)
	{
		bool bValidAnswer = false;
		ErrorMessage = "";

		if (Question.AnswerRequired)
		{
			switch (Question.QuestionType)
			{
				case Questionnaire.QuestionType.Response:
					if (ResponseDisplay.ResponseField.text != "")
					{
						bValidAnswer = true;
					}
					break;
				case Questionnaire.QuestionType.YesNo:
					if (YesNoDisplay.ToggleGroup.AnyTogglesOn ())
					{
						bValidAnswer = true;
					}
					break;
				case Questionnaire.QuestionType.Scale:
					break;
			}
			if (!bValidAnswer)
			{
				ErrorMessage = "The highlighted question needs to be answered to move on!";
			}
		}
		else
		{
			bValidAnswer = true;
		}

		return bValidAnswer;
	}

	/// <summary>
	/// Gets the response from the question.
	/// </summary>
	/// <returns>The string resonse.</returns>
	public string GetResponse ()
	{
		string retString = "";

		switch (Question.QuestionType)
		{
			case Questionnaire.QuestionType.Response:
				retString = ResponseDisplay.ResponseField.text;
				break;
			case Questionnaire.QuestionType.YesNo:
				if (YesNoDisplay.Yes.isOn)
				{
					retString = Questionnaire.YES;
				}
				else
				{
					retString = Questionnaire.NO;
				}
				break;
			case Questionnaire.QuestionType.Scale:
				break;
		}

		return retString;
	}

	/// <summary>
	/// Sets whether the UI is highlighted.
	/// </summary>
	/// <param name="bHighlighted">Whether or not to highlight the UI.</param>
	public void Highlight (bool bHighlighted)
	{
		if (bHighlighted)
		{
			Background.color = HighlightedColor;
		}
		else
		{
			Background.color = NormalColor;
		}
	}

	/// <summary>
	/// Returns the QuestionId for the displayed Question.
	/// </summary>
	/// <returns>The QuestionId.</returns>
	public int GetQuestionId ()
	{
		return Question.QuestionId;
	}
}
