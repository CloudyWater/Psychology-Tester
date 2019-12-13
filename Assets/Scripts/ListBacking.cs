/******************************************************************************
 * File: ListBacking.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the ListBacking class. It automatically resises
 * the RectTransform to be the required size of the VerticalLayoutGroup.
 * ***************************************************************************/
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (VerticalLayoutGroup))]
[RequireComponent (typeof (RectTransform))]
public class ListBacking : MonoBehaviour
{
	private RectTransform BackingTransform;
	private VerticalLayoutGroup LayoutGroup;

	/// <summary>
	/// Initializes the BackingTransform and VerticalLayoutGroup elements.
	/// </summary>
	private void Start ()
	{
		BackingTransform = GetComponent<RectTransform> ();
		LayoutGroup = GetComponent<VerticalLayoutGroup> ();
	}

	/// <summary>
	/// Sets the backing RectTransform to have a height equal to the combined height
	/// of the VerticalLayoutGroup's child elements + spacing + offsets.
	/// </summary>
	void Update ()
	{
		BackingTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, LayoutGroup.preferredHeight);
	}
}
