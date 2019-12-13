using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogManager : MonoBehaviour
{
	[System.Serializable]
	public struct MessageDialogUI
	{
		public RectTransform Panel;
		public Text TitleText, MessageText;

		public void DisplayMessage (string messageTitle, string messageText)
		{
			Panel.gameObject.SetActive (true);
			TitleText.text = messageTitle.ToUpper ();
			MessageText.text = messageText;
		}
	}

	[System.Serializable]
	public struct ConfirmationDialogUI
	{
		public RectTransform Panel;
		public Text TitleText, MessageText;
		public Button OkButton;

		public void DisplayConfirmation (string messageTitle, string messageText, System.Action confirmationAction)
		{
			OkButton.onClick.RemoveAllListeners ();
			OkButton.onClick.AddListener (delegate { confirmationAction (); });
			TitleText.text = messageTitle.ToUpper ();
			MessageText.text = messageText;
			Panel.gameObject.SetActive (true);
		}
	}

	public AddTrialDialogUI AddTrialDialog;
	public MessageDialogUI MessageDialog;
	public ConfirmationDialogUI ConfirmationDialog;
	public ColorSelectionPanelUI ColorSelectionPanel;

	/// <summary>
	/// Unity-Optimized Singleton pattern for easy popup window access.
	/// </summary>
	private static DialogManager instance;

	private DialogManager () { }

	public static DialogManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<DialogManager> ();
			}
			return instance;
		}
	}

	/// <summary>
	/// Called once at the start of operation. Sets up the static instance.
	/// If the static instance exists, self destructs.
	/// </summary>
	void Start ()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy (this.gameObject);
		}

		DontDestroyOnLoad (this.gameObject);
	}

	/// <summary>
	/// Displays the Add Trial Dialog box.
	/// </summary>
	public void ShowAddTrialDialog ()
	{
		AddTrialDialog.transform.parent.gameObject.SetActive (true);
	}

	public void ShowColorSelectionPanel (ColorPickerUI callingUI)
	{
		ColorSelectionPanel.transform.parent.gameObject.SetActive (true);
		ColorSelectionPanel.ShowDisplay (callingUI);
	}
}
