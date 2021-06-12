using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player1 = null;
    private GameObject player2 = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(player1 == null)
                player1 = Instantiate(playerPrefab);
            else
                Destroy(player1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(player2 == null)
            {
                player2 = Instantiate(playerPrefab);
                player2.GetComponent<PlayerInput>().isFirstPlayer = false;
            }
            else
                Destroy(player2);
        }
    }
}
