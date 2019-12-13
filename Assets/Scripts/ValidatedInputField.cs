using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

public class ValidatedInputField : InputField, IHighlightableUI, IValidatedUI
{
	/// <summary>
	/// Highlights the targetGraphic (background of text) with the ErrorHighlightColor
	/// or transitions back to the normal color.
	/// </summary>
	/// <param name="bIsHighlighted">Whether to highlight the field or not.</param>
	public void Highlight (bool bIsHighlighted)
	{
		if (bIsHighlighted)
		{
			targetGraphic.CrossFadeColor (colors.disabledColor, colors.fadeDuration, false, false);
		}
		else
		{
			targetGraphic.CrossFadeColor (colors.normalColor, colors.fadeDuration, false, false);
		}
	}

	/// <summary>
	/// Validates the text field. If validation fails, highlights the text and
	/// sends an error message back.
	/// </summary>
	/// <param name="ErrorMessage">Contains validation error information.</param>
	/// <returns>True if validated, false otherwise.</returns>
	public bool Validate (out string ErrorMessage)
	{
		bool bValidField = true;
		ErrorMessage = "";

		if (text == "")
		{
			bValidField = false;
			ErrorMessage = "Highlighted Input Field is Empty!";
			Highlight (true);
		}

		return bValidField;
	}
}
