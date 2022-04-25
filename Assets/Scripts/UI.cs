using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject player;
    public Text currentBiome;
    public Text currentCoords;

    // Start is called before the first frame update
    void Start()
    {
        currentBiome.text = "None";
        currentCoords.text = player.transform.position.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        currentBiome.text = Utils.WhichBiome((int)player.transform.position.x, (int)player.transform.position.z).ToString();
        currentCoords.text = player.transform.position.ToString();
    }
}
