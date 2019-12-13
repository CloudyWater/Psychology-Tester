/******************************************************************************
 * File: QuestionDesignUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the QuestionDesignUI class. It displays the
 * question setup UI and collects question information for the Questionnaire
 * Design UI.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

public class QuestionDesignUI : MonoBehaviour, IValidatedUI
{
	public Dropdown QuestionType;
	public Toggle RequiredAnswer;
	public ValidatedInputField QuestionInput;
	public Button RemoveQuestionButton;

	/// <summary>
	/// Sets up the UI with a Question and the QuestionnaireDesignUI parent.
	/// </summary>
	/// <param name="parent">The QuestionnaireDesignUI displaying this UI.</param>
	/// <param name="question">The Question to display.</param>
	public void InitializeUI (QuestionnaireDesignUI parent, Questionnaire.Question question)
	{
		RemoveQuestionButton.onClick.AddListener (delegate { parent.RemoveQuestion (this); });
		SetQuestion (question);
	}

	/// <summary>
	/// Returns the Question setup as displayed by this UI.
	/// </summary>
	/// <returns>A Question object.</returns>
	public Questionnaire.Question GetQuestion ()
	{
		Questionnaire.Question question = new Questionnaire.Question
		{
			QuestionText = QuestionInput.text,
			QuestionType = (Questionnaire.QuestionType) QuestionType.value,
			AnswerRequired = RequiredAnswer.isOn
		};
		return question;
	}

	/// <summary>
	/// Sets the question UI to display the passed in question.
	/// </summary>
	/// <param name="question">The question to display.</param>
	public void SetQuestion (Questionnaire.Question question)
	{
		QuestionType.value = (int) question.QuestionType;
		QuestionInput.text = question.QuestionText;
		RequiredAnswer.isOn = question.AnswerRequired;
	}

	/// <summary>
	/// Moves the question up in the list.
	/// </summary>
	public void MoveUp ()
	{
		if (transform.GetSiblingIndex () > 0)
		{
			transform.SetSiblingIndex (transform.GetSiblingIndex () - 1);
		}
	}

	/// <summary>
	/// Moves the question down in the list.
	/// </summary>
	public void MoveDown ()
	{
		if (transform.GetSiblingIndex () < transform.parent.childCount)
		{
			transform.SetSiblingIndex (transform.GetSiblingIndex () + 1);
		}
	}

	/// <summary>
	/// Validates the UI, making sure a question has been input.
	/// </summary>
	/// <param name="ErrorMessage">Returns an error message on validation failure.</param>
	/// <returns>True if UI validates, false otherwise.</returns>
	public bool Validate (out string ErrorMessage)
	{
		bool bValid = true;

		if (!QuestionInput.Validate (out ErrorMessage))
		{
			ErrorMessage = "There is no question text set for the highlighted question!";
			bValid = false;
		}

		return bValid;
	}
}
