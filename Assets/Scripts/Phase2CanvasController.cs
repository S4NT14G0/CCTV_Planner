using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Phase2CanvasController : MonoBehaviour {

    int budget;

    [SerializeField]
    GameObject cctvBtnPrefab;

    [SerializeField]
    GameObject buttonBank;

	InputField budgetInput;

	public delegate void OnCreateCamera ();
	public event OnCreateCamera onCreateCamera;


    const int pricePerCamera = 750;

	// Use this for initialization
	void Start () {
        budget = 0;
	}
	
    public void UpdateBank (InputField input)
    {
        budget = int.Parse(input.text);
        input.text = "Remaining $" + input.text;

        int cameras = budget / pricePerCamera;

        for (int i = 0; i < cameras; i++)
        {
            GameObject cam = GameObject.Instantiate(cctvBtnPrefab);
            Button button = cam.GetComponent<Button>();
            button.onClick.AddListener(() => { OnButtonClicked(button); });
			cam.transform.SetParent(buttonBank.transform);
        }

        input.enabled = false;
    }

    void OnButtonClicked (Button clickedButton)
    {
		budget -= pricePerCamera;

		// Notify if any UI is listening
		if (onCreateCamera != null)
			onCreateCamera();

		Destroy (clickedButton.gameObject);
    }

}
