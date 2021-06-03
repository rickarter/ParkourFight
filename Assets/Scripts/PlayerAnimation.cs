using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //Components
    public Animator animator;
    private Rigidbody2D rb;

    //Asignables
    public Transform overridenFrontArmEffector;
    public Transform frontArmEffector;

    public Transform overridenBackArmEffector;
    public Transform backArmEffector;

    //Grabing
    private bool grab = false;
    private Vector3 grabPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        OverrideAnimation();
    }

    void LateUpdate()
    {

    }

    void Animate()
    {
        
    }

    void OverrideAnimation()
    {
        if(grab)
        {
            frontArmEffector.position = grabPoint;
            backArmEffector.position = grabPoint;
        }
    }

    public void GrabAnimation(Vector3 point)
    {
        grab = true;
        grabPoint = point;
        overridenFrontArmEffector.parent.gameObject.SetActive(false);
        overridenBackArmEffector.parent.gameObject.SetActive(false);

        frontArmEffector.parent.gameObject.SetActive(true);
        backArmEffector.parent.gameObject.SetActive(true);
        Invoke(nameof(StopGrabAnimation), 0.2f);
    }

    void StopGrabAnimation()
    {
        grab = false;
            overridenFrontArmEffector.parent.gameObject.SetActive(true);
        overridenBackArmEffector.parent.gameObject.SetActive(true);

        frontArmEffector.parent.gameObject.SetActive(false);
        backArmEffector.parent.gameObject.SetActive(false);
    }
}
