/******************************************************************************
 * File: BouncingObject.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the BouncingObject class. It is a TrialObject
 * that moves around the screen diagonally and bounces when it hits the screen.
 * One set of objects will count the times they bounce during the test.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (SpriteRenderer))]
public class BouncingObject : TrialObject
{
	public enum ObjectPaths
	{
		Diagonal, Straight
	}

	public Vector2 [] DiagonalSpeeds;
	public Vector2 [] StraightSpeeds;

	private BasicTrialManager.BouncingObjectSettings Settings;
	private BasicTrialManager TrialManager;
	private Bounds CameraBounds;

	private Rigidbody2D Rigidbody2D;
	private BoxCollider2D BoxCollider;
	private SpriteRenderer ObjectSprite;

	private static int CountedBounces = 0;

	/// <summary>
	/// Returns the number of times the tracked objects bounced off the screen during the trial.
	/// </summary>
	/// <returns>The number of counted bounces as an integer.</returns>
	public static int GetNumberOfBounces ()
	{
		return CountedBounces;
	}

	/// <summary>
	/// Resets the number of counted bounces to zero.
	/// </summary>
	public static void ResetBounces ()
	{
		CountedBounces = 0;
	}

	/// <summary>
	/// Sets the objects settings based upon the passed in BouncingObjectSettings. Initializes all
	/// customizable variables and prepares for test start.
	/// </summary>
	/// <param name="settings">The BouncingObjectSettings to use.</param>
	/// <param name="camera">The camera used for the test, to determine screen area.</param>
	public void SetObjectSettings (BasicTrialManager.BouncingObjectSettings settings, Camera camera)
	{
		bool isIntersecting = true;
		Vector2 position = Vector2.zero;
		//Gets bounds to bounce off of.
		float screenAspect = (float) Screen.width / (float) Screen.height;
		float cameraHeight = camera.orthographicSize * 2;
		CameraBounds = new Bounds (camera.transform.position, new Vector3 (cameraHeight * screenAspect, cameraHeight, 0));

		Settings = settings;
		Rigidbody2D = GetComponent<Rigidbody2D> ();
		ObjectSprite = GetComponent<SpriteRenderer> ();
		BoxCollider = GetComponent<BoxCollider2D> ();

		ObjectSprite.sprite = GetObjectSprite (settings.Shape);
		ObjectSprite.color = Settings.Color.GetColor ();

		float objectScale = BASE_SIZE / ObjectSprite.sprite.rect.width;

		transform.localScale = new Vector3(objectScale, objectScale, 1) * Settings.Scale;

		float cameraY = CameraBounds.min.y;
		float cameraX = CameraBounds.min.x;

		Vector3 objSize = BoxCollider.bounds.size;

		while (isIntersecting)
		{
			position = new Vector2 (Random.Range (-cameraX + cameraX / 4, cameraX - cameraX / 4),
				Random.Range (-cameraY + cameraY / 4, cameraY - cameraY / 4));
			Collider2D hitObject = Physics2D.OverlapBox (position, objSize, 0);
			if (hitObject == null)
			{
				isIntersecting = false;
			}
		}

		Vector2 velocity = Vector2.zero;

		switch (Settings.Path)
		{
			case ObjectPaths.Diagonal:
				velocity = DiagonalSpeeds [Random.Range (0, DiagonalSpeeds.Length)] * Settings.Speed * OBJECT_BASE_SPEED;
				break;
			case ObjectPaths.Straight:
				velocity = StraightSpeeds [Random.Range (0, StraightSpeeds.Length)] * Settings.Speed * OBJECT_BASE_SPEED;
				break;
		}

		if (Settings.bTrackedObject)
		{
			if (position.x > 0 && velocity.x > 0 || position.x < 0 && velocity.x < 0)
			{
				velocity = new Vector2 (velocity.x * -1, velocity.y);
			}
			if (position.y > 0 && velocity.y > 0 || position.y < 0 && velocity.y < 0)
			{
				velocity = new Vector2 (velocity.x, velocity.y * -1);
			}
		}

		transform.position = position;
		Rigidbody2D.velocity = velocity;
	}

	/// <summary>
	/// Checks whether the object needs to bounce on either the x or y axis. If it does,
	/// reverses the direction variable and increments bounce counter if it is a tracked object.
	/// </summary>
	public void FixedUpdate ()
	{
		if (transform.position.y + ObjectSprite.bounds.extents.y > CameraBounds.max.y ||
			transform.position.y - ObjectSprite.bounds.extents.y < CameraBounds.min.y)
		{
			Rigidbody2D.velocity = new Vector2 (Rigidbody2D.velocity.x, -Rigidbody2D.velocity.y);

			if (Settings.bTrackedObject)
			{
				CountedBounces++;
				Debug.Log ("Bounces: " + CountedBounces);
			}

			if (transform.position.y > 0)
			{
				transform.position = new Vector2 (transform.position.x, CameraBounds.max.y - ObjectSprite.bounds.extents.y);
			}
			else
			{
				transform.position = new Vector2 (transform.position.x, CameraBounds.min.y + ObjectSprite.bounds.extents.y);
			}
		}

		if (transform.position.x + ObjectSprite.bounds.extents.x > CameraBounds.max.x ||
			transform.position.x - ObjectSprite.bounds.extents.x < CameraBounds.min.x)
		{
			Rigidbody2D.velocity = new Vector2 (-Rigidbody2D.velocity.x, Rigidbody2D.velocity.y);

			if (Settings.bTrackedObject)
			{
				CountedBounces++;
				Debug.Log ("Bounces: " + CountedBounces);
			}

			if (transform.position.x > 0)
			{
				transform.position = new Vector2 (CameraBounds.max.x - ObjectSprite.bounds.extents.x, transform.position.y);
			}
			else
			{
				transform.position = new Vector2 (CameraBounds.min.x + ObjectSprite.bounds.extents.x, transform.position.y);
			}
		}
	}
}
