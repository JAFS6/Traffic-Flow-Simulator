using UnityEngine;
using System.Collections;

public class MainMenuButtonsController : MonoBehaviour {

	public void showStart () {
		// Ocultar los otros paneles
		hideOptionsPanel();
		hideCreditsPanel();
		// Mostrar el panel de inicio
		showStartPanel();
	}
	
	public void showOptions () {
		// Ocultar los otros paneles
		hideStartPanel();
		hideCreditsPanel();
		// Mostrar el panel de opciones
		showOptionsPanel();
	}
	
	public void showCredits () {
		// Ocultar los otros paneles
		hideStartPanel();
		hideOptionsPanel();
		// Mostrar el panel de opciones
		showCreditsPanel();
	}
	
	public void exitApplication () {
		hideStartPanel();
		hideOptionsPanel();
		hideCreditsPanel();
		Application.Quit();
	}
	
	private void showStartPanel () {
		GameObject start_panel = GameObject.Find("Start Panel");
		Vector3 pos = start_panel.transform.position;
		pos.x = 350;
		start_panel.transform.position = pos;
	}
	
	private void showOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		Vector3 pos = options_panel.transform.position;
		pos.x = 350;
		options_panel.transform.position = pos;
	}
	
	private void showCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		Vector3 pos = credits_panel.transform.position;
		pos.x = 350;
		credits_panel.transform.position = pos;
	}
	
	private void hideStartPanel () {
		GameObject start_panel = GameObject.Find("Start Panel");
		Vector3 pos = start_panel.transform.position;
		pos.x = -600;
		start_panel.transform.position = pos;
	}
	
	private void hideOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		Vector3 pos = options_panel.transform.position;
		pos.x = -600;
		options_panel.transform.position = pos;
	}
	
	private void hideCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		Vector3 pos = credits_panel.transform.position;
		pos.x = -600;
		credits_panel.transform.position = pos;
	}
}
