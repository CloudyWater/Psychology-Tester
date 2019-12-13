/******************************************************************************
 * File: ColorSelectionPanelUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the ColorPickerUI class. This class allows for
 * picking a custom color using three input fields and 0-255 RGB input.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionPanelUI : MonoBehaviour
{
	public Image ColorDisplay;

	public ColorSelectionUI RedSelection, GreenSelection, BlueSelection;

	private ColorPickerUI SelectingUI;

	/// <summary>
	/// Called when the color values are updated. Changes the displayed
	/// color in the ColorDisplay.
	/// </summary>
	public void ColorUpdated ()
	{
		TrialManager.JsonColor color = new TrialManager.JsonColor
		{
			R = RedSelection.GetColorValue (),
			G = GreenSelection.GetColorValue (),
			B = BlueSelection.GetColorValue ()
		};

		ColorDisplay.color = color.GetColor ();
	}

	/// <summary>
	/// Sets the initial color of the display, and the ColorPickerUI that is
	/// changing colors.
	/// </summary>
	/// <param name="pickerUI">The UI that asked to show the display.</param>
	public void ShowDisplay (ColorPickerUI pickerUI)
	{
		SelectingUI = pickerUI;
		ColorDisplay.color = SelectingUI.GetJSONColor ().GetColor ();
		RedSelection.SetColorValue (SelectingUI.GetJSONColor ().R);
		GreenSelection.SetColorValue (SelectingUI.GetJSONColor ().G);
		BlueSelection.SetColorValue (SelectingUI.GetJSONColor ().B);
	}

	/// <summary>
	/// Sets the color of the UI that showed this display to the currently
	/// selected color.
	/// </summary>
	public void ConfirmColorChange ()
	{
		TrialManager.JsonColor color = new TrialManager.JsonColor
		{
			R = RedSelection.GetColorValue (),
			G = GreenSelection.GetColorValue (),
			B = BlueSelection.GetColorValue ()
		};

		SelectingUI.SetJSONColor (color);
	}
}
