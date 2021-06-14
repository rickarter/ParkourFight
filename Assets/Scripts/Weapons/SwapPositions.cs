using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapPositions : Weapon
{
    public GameObject particles;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<PlayerScript>();
        player.weapon = this;
    } 

    public override void Fire(MyInput input)
    {
        SwapPosition();
        base.Fire(input);
    }

    public void SwapPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        Instantiate(particles, players[0].transform.position, Quaternion.identity);
        Instantiate(particles, players[1].transform.position, Quaternion.identity);

        Vector3 bufferPosition = players[0].transform.position;
        players[0].transform.position = players[1].transform.position;
        players[1].transform.position = bufferPosition;
        
        Quaternion bufferRotation = players[0].transform.rotation;
        players[0].transform.rotation = players[1].transform.rotation;
        players[1].transform.rotation = bufferRotation;
    }

    public override void Rotate(Vector2 direction)
    {
        Vector3 shoulderOffset = player.playerMovement.shoulderOffset;
        shoulderOffset = transform.up * shoulderOffset.y + transform.right * shoulderOffset.x;

        Vector3 shoulderPos = shoulderOffset + player.transform.position;
        Vector3 handPos = !player.playerAnimation.frontArmEffectorO.parent.gameObject.activeSelf ? player.playerAnimation.frontArmEffector.position : player.playerAnimation.frontArmEffectorO.position;

        Vector3 distance = handPos - shoulderPos;
        Debug.DrawLine(shoulderPos, handPos);
        if(distance.magnitude > player.playerMovement.grabRadius)
            distance = distance.normalized * player.playerMovement.grabRadius;
        transform.position = shoulderPos + distance;
    }
}
