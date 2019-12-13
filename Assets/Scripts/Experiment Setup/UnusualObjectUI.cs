/******************************************************************************
 * File: UnusualObjectUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the UnusualObjectUI class. It displays the
 * customizable options for UnusualObjects.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnusualObjectUI : MonoBehaviour
{
	public InputField Speed, Scale, SpawnTime;
	public Dropdown Shape, Path;
	public Button RemoveButton;
	public ColorPickerUI ColorPicker;

	public Image ObjectPreview;

	/// <summary>
	/// Initializes the UI with settings and the BasicTrialObjectDesignUI parent.
	/// </summary>
	/// <param name="parent">The parent UI displaying this UI.</param>
	/// <param name="settings">The Settings to initialize this display with.</param>
	public void InitializeUI (BasicTrialObjectDesignUI parent, BasicTrialManager.UnusualObjectSettings settings)
	{
		RemoveButton.onClick.AddListener (delegate { parent.RemoveTrialObject (this); });
		SetUI (settings);
	}

	/// <summary>
	/// Retrieves the UnuualObjetSettings as displayed in the UI.
	/// </summary>
	/// <returns>An UnusualObjectSettings filled by the UI.</returns>
	public BasicTrialManager.UnusualObjectSettings GetSettings ()
	{
		BasicTrialManager.UnusualObjectSettings settings = new BasicTrialManager.UnusualObjectSettings
		{
			Speed = float.Parse (Speed.text),
			Scale = float.Parse (Scale.text),
			SpawnTime = float.Parse (SpawnTime.text),
			Color = ColorPicker.GetJSONColor (),
			Shape = (UnusualObject.ObjectShapes) Shape.value,
			Path = (UnusualObject.ObjectPaths) Path.value
		};

		return settings;
	}

	/// <summary>
	/// Sets the UI to display the passed in UnusualObjectSettings.
	/// </summary>
	/// <param name="settings">The settings to display.</param>
	public void SetUI (BasicTrialManager.UnusualObjectSettings settings)
	{
		Speed.text = settings.Speed.ToString ();
		Scale.text = settings.Scale.ToString ();
		SpawnTime.text = settings.SpawnTime.ToString ();
		ColorPicker.SetJSONColor (settings.Color);
		Shape.value = (int) settings.Shape;
		Path.value = (int) settings.Path;

		SetPreviewColor ();
		SetPreviewImage ();
	}

	/// <summary>
	/// Sets the color of the object's preview image.
	/// </summary>
	public void SetPreviewColor ()
	{
		ObjectPreview.color = ColorPicker.GetJSONColor ().GetColor ();
	}

	/// <summary>
	/// Sets the sprite of the object's preveiew image.
	/// </summary>
	public void SetPreviewImage ()
	{
		ObjectPreview.sprite = UnusualObject.GetObjectSprite ((UnusualObject.ObjectShapes) Shape.value);
	}
}
