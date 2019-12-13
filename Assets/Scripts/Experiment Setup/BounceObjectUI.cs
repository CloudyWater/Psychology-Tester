/******************************************************************************
 * File: BounceObjectUI.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the BounceObjectUI class. It displays the
 * customizable parts of a BouncingObject for pre-test design.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Image))]
public class BounceObjectUI : MonoBehaviour
{
	public InputField Speed, Scale, NumSpawned;
	public Dropdown Shape, Path;
	public Toggle Tracked;
	public Button RemoveButton;
	public ColorPickerUI ColorPicker;

	public Image ObjectPreview;

	/// <summary>
	/// Initializes the UI with both BouncingObjectSettings and the parent BasicTrialObjectDesignUI
	/// </summary>
	/// <param name="parentUI">The BasicTrialObjectDesignUI parent.</param>
	/// <param name="settings">The beginning settings of the object.</param>
	public void InitializeUI (BasicTrialObjectDesignUI parentUI, BasicTrialManager.BouncingObjectSettings settings)
	{
		RemoveButton.onClick.AddListener (delegate { parentUI.RemoveTrialObject (this); });
		Tracked.onValueChanged.AddListener (delegate { parentUI.Highlight (false); });
		SetUI (settings);
	}

	/// <summary>
	/// Collects the BouncingObjectSettings as designed in the UI.
	/// </summary>
	/// <returns>A BouncingObjectSettings representing the BouncingObject.</returns>
	public BasicTrialManager.BouncingObjectSettings GetObjectSettings ()
	{
		BasicTrialManager.BouncingObjectSettings settings = new BasicTrialManager.BouncingObjectSettings
		{
			bTrackedObject = Tracked.isOn,
			Scale = float.Parse (Scale.text),
			Speed = float.Parse (Speed.text),
			NumberToSpawn = int.Parse (NumSpawned.text),
			Color = ColorPicker.GetJSONColor (),
			Shape = (BouncingObject.ObjectShapes) Shape.value,
			Path = (BouncingObject.ObjectPaths) Path.value
		};

		return settings;
	}

	/// <summary>
	/// Sets the UI to reflect the BouncingObjectSettings as passed in.
	/// </summary>
	/// <param name="settings">The BouncingObjectSettings to display.</param>
	public void SetUI (BasicTrialManager.BouncingObjectSettings settings)
	{
		Speed.text = settings.Speed.ToString ();
		Scale.text = settings.Scale.ToString ();
		NumSpawned.text = settings.NumberToSpawn.ToString ();
		ColorPicker.SetJSONColor (settings.Color);
		Tracked.isOn = settings.bTrackedObject;
		Shape.value = (int) settings.Shape;
		Path.value = (int) settings.Path;

		SetPreviewImage ();
		SetPreviewColor ();
	}

	/// <summary>
	/// Sets the color of the object's preview.
	/// </summary>
	public void SetPreviewColor ()
	{
		ObjectPreview.color = ColorPicker.GetJSONColor().GetColor ();
	}

	/// <summary>
	/// Sets the image of the object's preview.
	/// </summary>
	public void SetPreviewImage ()
	{
		ObjectPreview.sprite = BouncingObject.GetObjectSprite ((BouncingObject.ObjectShapes) Shape.value);
	}

	/// <summary>
	/// Sets the speed and scale of the object.
	/// </summary>
	/// <param name="speed">Speed of the object.</param>
	/// <param name="scale">Scale of the object.</param>
	public void SetSpeedScale (float speed, float scale)
	{
		Speed.text = speed.ToString ();
		Scale.text = scale.ToString ();
	}
}
