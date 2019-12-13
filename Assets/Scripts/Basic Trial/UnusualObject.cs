/******************************************************************************
 * File: UnusualObject.cs
 * Author: Thomas Hyman
 * Purpose: This file contains the UnusualObject class. UnusualObjects move
 * across the screen in a different fashion than the BouncingObjects do. For
 * the basic test, it simply moves across the screen in a horizontal line.
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (SpriteRenderer))]
public class UnusualObject : TrialObject
{
	public enum ObjectPaths
	{
		Vertical, Horizontal, Diagonal
	}

	public Vector2 [] HorizontalSpawns;

	public Vector2 [] VerticalSpawns;

	private SpriteRenderer SpriteRenderer;
	private Rigidbody2D Rigidbody2D;
	private Bounds CameraBounds;

	private BasicTrialManager.UnusualObjectSettings Settings;

	private float DisplayTimer;
	private bool TimeDisplayed;

	private void Start ()
	{
		DisplayTimer = 0.0f;
		TimeDisplayed = false;
	}

	/// <summary>
	/// If the path is diagonal, bounce it off the top/bottom of the screen.
	/// </summary>
	public void FixedUpdate ()
	{
		if (Settings.Path.Equals (ObjectPaths.Diagonal))
		{
			if (transform.position.y + SpriteRenderer.bounds.extents.y > CameraBounds.max.y ||
				transform.position.y - SpriteRenderer.bounds.extents.y < CameraBounds.min.y)
			{
				Rigidbody2D.velocity = new Vector2 (Rigidbody2D.velocity.x, -Rigidbody2D.velocity.y);

				if (transform.position.y > 0)
				{
					transform.position = new Vector2 (transform.position.x, CameraBounds.max.y - SpriteRenderer.bounds.extents.y);
				}
				else
				{
					transform.position = new Vector2 (transform.position.x, CameraBounds.min.y + SpriteRenderer.bounds.extents.y);
				}
			}
		}
		if (CameraBounds.Contains (transform.position))
		{
			DisplayTimer += Time.deltaTime;
		}
		else if (!TimeDisplayed && DisplayTimer > 1.0f)
		{
			Debug.Log ("Unusual Object displayed for " + DisplayTimer + " seconds.");
			TimeDisplayed = true;
		}
	}

	/// <summary>
	/// Sets the settings of the object.
	/// </summary>
	/// <param name="settings">The UnusualObjectSettings to use.</param>
	/// <param name="camera">The camera to use to determine spawn position.</param>
	public void SetObjectSettings (BasicTrialManager.UnusualObjectSettings settings, Camera camera)
	{
		Settings = settings;
		float screenAspect = (float) Screen.width / (float) Screen.height;
		float cameraHeight = camera.orthographicSize * 2;
		CameraBounds = new Bounds (camera.transform.position, new Vector3 (cameraHeight * screenAspect, cameraHeight, 0));

		SpriteRenderer = GetComponent<SpriteRenderer> ();
		Rigidbody2D = GetComponent<Rigidbody2D> ();

		SpriteRenderer.sprite = GetObjectSprite (settings.Shape);
		SpriteRenderer.color = settings.Color.GetColor ();

		float objectScale = BASE_SIZE / SpriteRenderer.sprite.rect.width;

		transform.localScale = new Vector3 (objectScale, objectScale, 1) * Settings.Scale;

		SetBasicSpeedAndPosition (cameraHeight, screenAspect);
	}

	/// <summary>
	/// Sets the speed and starting position for the basic set of objects.
	/// </summary>
	/// <param name="cameraHeight">The size of the screen height.</param>
	/// <param name="screenAspect">The screen aspect.</param>
	private void SetBasicSpeedAndPosition (float cameraHeight, float screenAspect)
	{
		int spawnPos;

		switch (Settings.Path)
		{
			case ObjectPaths.Horizontal:
				spawnPos = Random.Range (0, HorizontalSpawns.Length);
				transform.position = new Vector2 (cameraHeight * screenAspect / 2 * HorizontalSpawns [spawnPos].x, cameraHeight / 2 * HorizontalSpawns [spawnPos].y);
				if (transform.position.x < 0)
				{
					Rigidbody2D.velocity = new Vector2 (Settings.Speed, 0);
				}
				else
				{
					Rigidbody2D.velocity = new Vector2 (-Settings.Speed, 0);
				}
				break;
			case ObjectPaths.Vertical:
				spawnPos = Random.Range (0, VerticalSpawns.Length);
				transform.position = new Vector2 (cameraHeight * screenAspect / 2 * VerticalSpawns [spawnPos].x, cameraHeight / 2 * VerticalSpawns [spawnPos].y);
				if (transform.position.y > 0)
				{
					Rigidbody2D.velocity = new Vector2 (0, -Settings.Speed);
					transform.rotation = Quaternion.Euler (0, 0, 180);
				}
				else
				{
					Rigidbody2D.velocity = new Vector2 (0, Settings.Speed);
				}
				break;
			case ObjectPaths.Diagonal:
				spawnPos = Random.Range (0, HorizontalSpawns.Length);
				transform.position = new Vector2 (cameraHeight * screenAspect / 2 * HorizontalSpawns [spawnPos].x, cameraHeight / 2 * HorizontalSpawns [spawnPos].y);
				float ySpeed, xSpeed;
				if (transform.position.y > 0)
				{
					ySpeed = -Settings.Speed;
				}
				else
				{
					ySpeed = Settings.Speed;
				}
				if (transform.position.x > 0)
				{
					xSpeed = -Settings.Speed;
				}
				else
				{
					xSpeed = Settings.Speed;
				}
				Rigidbody2D.velocity = new Vector2 (xSpeed, ySpeed);
				break;
		}
		Rigidbody2D.velocity = Rigidbody2D.velocity.normalized * Settings.Speed * OBJECT_BASE_SPEED;
	}
}
