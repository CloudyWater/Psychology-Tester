/******************************************************************************
 * File: TrialObject.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the abstract TrialObject class. It contains some
 * Trial Object definitions.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrialObject : MonoBehaviour
{
	protected const string IMAGE_LOCATION = "Images/";
	protected const float OBJECT_BASE_SPEED = .6f;
	protected const float BASE_SIZE = 32.0f;

	protected const string CROSS = "CrossShape";
	protected const string XSHAPE = "XShape";
	protected const string OSHAPE = "OShape";
	protected const string LSHAPE = "LShape";
	protected const string TSHAPE = "TShape";
	protected const string SPIDER = "Spider";
	protected const string SPIDERTWO = "Spider2";
	protected const string BASKETBALL = "Basketball";
	protected const string LADYBUG = "Ladybug";
	protected const string GORILLA = "Gorilla";

	public enum ObjectShapes
	{
		Cross, Spider, SpiderTwo, XShape, OShape, LShape, TShape, Basketball, Ladybug, Gorilla
	}

	/// <summary>
	/// Defines the different types of TrialObjects.
	/// </summary>
	public enum ObjectType
	{
		BounceObject, UnusualObject
	}

	/// <summary>
	/// Returns the sprite displayed for the passed in ObjectShapes.
	/// </summary>
	/// <param name="objectShape">The ObjectShapes representing the shape of the bouncing object.</param>
	/// <returns>A sprite loaded from resources.</returns>
	public static Sprite GetObjectSprite (ObjectShapes objectShape)
	{
		Sprite retSprite = null;

		switch (objectShape)
		{
			case ObjectShapes.LShape:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + LSHAPE);
				break;
			case ObjectShapes.TShape:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + TSHAPE);
				break;
			case ObjectShapes.XShape:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + XSHAPE);
				break;
			case ObjectShapes.OShape:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + OSHAPE);
				break;
			case ObjectShapes.Spider:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + SPIDER);
				break;
			case ObjectShapes.Cross:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + CROSS);
				break;
			case ObjectShapes.Basketball:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + BASKETBALL);
				break;
			case ObjectShapes.Ladybug:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + LADYBUG);
				break;
			case ObjectShapes.SpiderTwo:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + SPIDERTWO);
				break;
			case ObjectShapes.Gorilla:
				retSprite = Resources.Load<Sprite> (IMAGE_LOCATION + GORILLA);
				break;
		}

		return retSprite;
	}
}
