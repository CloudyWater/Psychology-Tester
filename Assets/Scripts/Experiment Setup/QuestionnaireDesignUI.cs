/******************************************************************************
 * File: QuestionnaireDesignUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the QuestionnaireDesignUI class. It shows a list
 * of QuestionDesignUI that consist of the post-test questionaire.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

[RequireComponent (typeof (Image))]
public class QuestionnaireDesignUI : MonoBehaviour, IValidatedUI, IHighlightableUI
{
	public Dropdown QuestionSelectionDropdown;
	public VerticalLayoutGroup QuestionDisplayLayout;

	public QuestionDesignUI QuestionDesignUIPrefab;
	public Color NormalColor, HighlightedColor;

	private Image Background;

	/// <summary>
	/// Called once on startup. Sets the background image.
	/// </summary>
	void Start ()
	{
		Background = GetComponent<Image> ();
	}

	/// <summary>
	/// Adds a question depending on the type displayed in the QuestionSelectionDropdown element.
	/// </summary>
	public void AddQuestion ()
	{
		QuestionDesignUI ui = GameObject.Instantiate <QuestionDesignUI> (QuestionDesignUIPrefab);
		ui.InitializeUI (this, new Questionnaire.Question { QuestionText = "", QuestionType = (Questionnaire.QuestionType) QuestionSelectionDropdown.value });
		ui.GetComponent<RectTransform> ().SetParent (QuestionDisplayLayout.transform, false);
		StartCoroutine (ScrollList ());
	}

	/// <summary>
	/// Waits for the next frame before scrolling the display list to show the most recently added object.
	/// </summary>
	public IEnumerator ScrollList ()
	{
		yield return null;
		RectTransform displayTransform = QuestionDisplayLayout.GetComponent<RectTransform> ();
		displayTransform.localPosition = new Vector2 (displayTransform.localPosition.x, displayTransform.rect.height);
	}


	/// <summary>
	/// Adds a QuestionDesignUI based on the passed in Question.
	/// </summary>
	/// <param name="question">The Question to add.</param>
	public void AddQuestion (Questionnaire.Question question)
	{
		QuestionDesignUI ui = GameObject.Instantiate<QuestionDesignUI> (QuestionDesignUIPrefab);
		ui.InitializeUI (this, question);
		ui.GetComponent<RectTransform> ().SetParent (QuestionDisplayLayout.transform, false);
	}
	
	/// <summary>
	/// Removes the passed in question from the question list.
	/// </summary>
	/// <param name="question">The question to remove.</param>
	public void RemoveQuestion (QuestionDesignUI question)
	{
		Destroy (question.gameObject);
	}

	/// <summary>
	/// Returns the full list of displayed questions.
	/// </summary>
	/// <returns>A List of Questions as displayed in the UI.</returns>
	public List<Questionnaire.Question> GetQuestions ()
	{
		List<Questionnaire.Question> questions = new List<Questionnaire.Question> ();

		foreach (Transform child in QuestionDisplayLayout.transform)
		{
			QuestionDesignUI ui = child.GetComponent<QuestionDesignUI> ();
			questions.Add (ui.GetQuestion ());
		}

		return questions;
	}

	/// <summary>
	/// Validates the current questionnaire design.
	/// </summary>
	/// <param name="ErrorMessage">Contains error information.</param>
	/// <returns>True if validation succeeds, false otherwise.</returns>
	public bool Validate (out string ErrorMessage)
	{
		bool bValidated = true;
		ErrorMessage = "";

		if (QuestionDisplayLayout.transform.childCount == 0)
		{
			ErrorMessage = "There are no questions in the questionnaire for the selected trial!";
			bValidated = false;
			Highlight (true);
		}
		else
		{
			foreach (Transform child in QuestionDisplayLayout.transform)
			{
				QuestionDesignUI questionUI = child.GetComponent<QuestionDesignUI> ();
				if (!questionUI.Validate (out ErrorMessage))
				{
					bValidated = false;
					break;
				}
			}
		}

		return bValidated;
	}

	/// <summary>
	/// Highlights or un-highlights the UI element.
	/// </summary>
	/// <param name="isHighlighted">Whether to highlight the UI.</param>
	public void Highlight (bool isHighlighted)
	{
		if (isHighlighted)
		{
			Background.color = HighlightedColor;
		}
		else
		{
			Background.color = NormalColor;
		}
	}
}
