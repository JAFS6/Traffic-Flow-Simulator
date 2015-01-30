using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	
	public void loadMap (string filename) {
		Application.LoadLevel("Simulation");
	}
	
	public void showStart () {
		// Eliminar los posibles botones de carga de mapa anteriores
		GameObject [] buttons = GameObject.FindGameObjectsWithTag ("LoadMapButton");
		
		foreach (GameObject item in buttons) {
			Destroy(item);
		}
	
		// Ocultar los otros paneles
		hideOptionsPanel();
		hideCreditsPanel();
		// Mostrar el panel de inicio
		showStartPanel();
		
		// Buscar los mapas disponibles y añadir un boton por cada uno de ellos
		DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/Resources/Maps/");
		FileInfo [] fileInfo = info.GetFiles();
		float i = 0;
		
		foreach (FileInfo file in fileInfo) {
			if (!file.Name.Contains("meta")) {
				string filename_w_extension = file.Name;
				string [] split = filename_w_extension.Split(new char[] {'.'});
				string filename = split[0];
				GameObject start_panel = GameObject.Find("Start Panel");
				
				GameObject button_prefab = Resources.Load("Prefabs/LoadMapButton", typeof(GameObject)) as GameObject;
				GameObject button = (GameObject)GameObject.Instantiate(button_prefab,new Vector3(0,0,0),Quaternion.identity);
				button.transform.SetParent(start_panel.transform.FindChild("MapLoadButtons").transform,false);
				button.name = "LoadMapButton"+"_"+i;
				button.tag = "LoadMapButton";
				button.GetComponentInChildren<Text>().text = filename;
				button.GetComponent<Button>().onClick.AddListener(delegate { loadMap(filename); });
				i++;
			}
		}
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
		start_panel.GetComponent<CanvasGroup>().interactable = true;
		start_panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}
	
	private void showOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		options_panel.GetComponent<CanvasGroup>().alpha = 1;
		options_panel.GetComponent<CanvasGroup>().interactable = true;
		options_panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}
	
	private void showCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		credits_panel.GetComponent<CanvasGroup>().alpha = 1;
	}
	
	private void hideStartPanel () {
		GameObject start_panel = GameObject.Find("Start Panel");
		start_panel.GetComponent<CanvasGroup>().alpha = 0;
		start_panel.GetComponent<CanvasGroup>().interactable = false;
		start_panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	private void hideOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		options_panel.GetComponent<CanvasGroup>().alpha = 0;
		options_panel.GetComponent<CanvasGroup>().interactable = false;
		options_panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	private void hideCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		credits_panel.GetComponent<CanvasGroup>().alpha = 0;
	}
}
