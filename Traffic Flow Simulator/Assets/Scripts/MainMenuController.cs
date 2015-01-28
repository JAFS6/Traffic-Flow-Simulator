using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

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
		start_panel.GetComponent<CanvasGroup>().alpha = 1;
	}
	
	private void showOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		options_panel.GetComponent<CanvasGroup>().alpha = 1;
	}
	
	private void showCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		credits_panel.GetComponent<CanvasGroup>().alpha = 1;
	}
	
	private void hideStartPanel () {
		GameObject start_panel = GameObject.Find("Start Panel");
		start_panel.GetComponent<CanvasGroup>().alpha = 0;
	}
	
	private void hideOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		options_panel.GetComponent<CanvasGroup>().alpha = 0;
	}
	
	private void hideCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		credits_panel.GetComponent<CanvasGroup>().alpha = 0;
	}
}
