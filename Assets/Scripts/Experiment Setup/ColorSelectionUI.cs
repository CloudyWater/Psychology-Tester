using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionUI : MonoBehaviour
{
	public ColorSelectionPanelUI ColorSelectionPanel;

	public Slider ColorSlider;
	public InputField ColorInput;

	/// <summary>
	/// Called when the slider's value is changed to update the value displayed in the text
	/// field and change the displayed color in the ColorSelectionPanel.
	/// </summary>
	public void OnSliderValueChanged ()
	{
		ColorInput.text = ColorSlider.value.ToString ();
		ColorSelectionPanel.ColorUpdated ();
	}

	/// <summary>
	/// Called when an edit is ended in the ColorInput input field to update the value of the
	/// color slider and change the displayed color in the ColorSelectionPanel.
	/// </summary>
	public void OnColorInputEndEdit ()
	{
		ColorSlider.value = int.Parse (ColorInput.text);
		ColorSelectionPanel.ColorUpdated ();
	}

	/// <summary>
	/// Returns the current color value as an int.
	/// </summary>
	/// <returns>The current color value.</returns>
	public int GetColorValue ()
	{
		return (int)ColorSlider.value;
	}

	/// <summary>
	/// Sets the color value to the passed in value.
	/// </summary>
	/// <param name="colorValue">The color value to set.</param>
	public void SetColorValue (int colorValue)
	{
		ColorSlider.value = colorValue;
	}
}
