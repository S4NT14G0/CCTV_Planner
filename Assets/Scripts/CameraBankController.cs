using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBankController : MonoBehaviour {

    int budget;

    [SerializeField]
    GameObject cctvBtnPrefab;

    [SerializeField]
    GameObject buttonBank;

    const int pricePerCamera = 750;

	// Use this for initialization
	void Start () {
        budget = 0;
	}
	
    public void UpdateBank (InputField input)
    {
        budget = int.Parse(input.text);
        input.text = "$" + input.text;

        int cameras = budget / pricePerCamera;

        for (int i = 0; i < cameras; i++)
        {
            GameObject cam = GameObject.Instantiate(cctvBtnPrefab);
            Button button = cam.GetComponent<Button>();
            button.onClick.AddListener(() => { OnButtonClicked(button); });
            cam.transform.parent = buttonBank.transform;
        }

        Debug.Log(cameras);
        input.enabled = false;
    }

    void OnButtonClicked (Button clickedButton)
    {
        Destroy(clickedButton);

    }

}
