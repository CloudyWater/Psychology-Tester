/******************************************************************************
 * File: ColorPickerUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the ColorPickerUI class. This class allows for
 * picking a custom color using three input fields and 0-255 RGB input.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloudyWaterGames.Interfaces;

public class ColorPickerUI : MonoBehaviour
{
	public Color DefaultColor;
	public Image ColorDisplay;

	private TrialManager.JsonColor Color;

	/// <summary>
	/// Called on Awake to set the default
	/// </summary>
	private void Awake ()
	{
		TrialManager.JsonColor defaultColor = new TrialManager.JsonColor
		{
			R = (int) TrialManager.JsonColor.Map (0, 255, 0, 1, DefaultColor.r),
			G = (int) TrialManager.JsonColor.Map (0, 255, 0, 1, DefaultColor.g),
			B = (int) TrialManager.JsonColor.Map (0, 255, 0, 1, DefaultColor.b)
		};
		SetJSONColor (defaultColor);
	}

	/// <summary>
	/// Displays the Color Selection Panel.
	/// </summary>
	public void ShowColorSelectionPanel ()
	{
		DialogManager.Instance.ShowColorSelectionPanel (this);
	}

	/// <summary>
	/// Gets the JSONColor as set up in the display.
	/// </summary>
	/// <returns>A JSONColor from the text entry.</returns>
	public TrialManager.JsonColor GetJSONColor ()
	{
		return Color;
	}

	/// <summary>
	/// Sets the JSONColor to the passed in color.
	/// </summary>
	/// <param name="color">The color to display.</param>
	public void SetJSONColor (TrialManager.JsonColor color)
	{
		Color = color;
		ColorDisplay.color = Color.GetColor ();
	}
}
