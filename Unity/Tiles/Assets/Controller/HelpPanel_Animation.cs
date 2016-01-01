using UnityEngine;
using System.Collections;

public class HelpPanel_Animation : MonoBehaviour {

	bool ShowingHelp = false;

	public GameObject RemoveThisPanel;

	public void ToggleShowHelp (Animator animator) {
		ShowingHelp = !ShowingHelp;
		animator.SetBool ("ShowHelp", ShowingHelp);
		if (RemoveThisPanel != null)
			RemoveThisPanel.SetActive (!ShowingHelp);
	
	}
}
