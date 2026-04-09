using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public float v = 0.0f;
    public float h = 0.0f;
    public float moveSpeed = 1.0f;
    public Transform PlayerTr;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        PlayerTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        animator.SetFloat("v", v);
        PlayerTr.Translate(new Vector3(h,0,v)*moveSpeed*Time.deltaTime);
    }
}
