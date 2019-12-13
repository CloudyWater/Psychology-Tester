/******************************************************************************
 * File: ObjectPreveiw.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the ObjectPreview class, which displays a preview
 * of what a test object looks like.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPreview : MonoBehaviour
{
	public Image PreviewImage;

	/// <summary>
	/// Sets the image to display.
	/// </summary>
	/// <param name="image">The sprite to display.</param>
	/// <param name="color">The color tint for the sprite.</param>
	public void SetImage (Sprite image, Color color)
	{
		PreviewImage.color = color;
		PreviewImage.sprite = image;
	}
}
