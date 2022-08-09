using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBehaviour : EntityBehaviour
{
    public float walkSpeed = 5;
    [SerializeField] private float t = 0;
    [SerializeField] private float actionTime = 0;
    [SerializeField] private ActionType action;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animal didn't have animator");
            Destroy(gameObject);
        }
    }

    public override void Update()
    {
        base.Update();

        if (t >= actionTime)
        {
            DoRandomAction();
        }

        t += Time.deltaTime;

        switch (action)
        {
            case ActionType.Walk:
                animator.SetBool("walking", true);
                transform.position += transform.forward * walkSpeed * Time.deltaTime;
                break;
            case ActionType.RotateLeft:
                animator.SetBool("walking", false);
                transform.Rotate(0,150 * walkSpeed * Time.deltaTime, 0);
                break;
            case ActionType.RotateRight:
                animator.SetBool("walking", false);
                transform.Rotate(0, -150 * walkSpeed * Time.deltaTime, 0);
                break;
            case ActionType.Still:
                animator.SetBool("walking", false);
                break;
        }
    }

    void DoRandomAction()
    {
        actionTime = Random.Range(1.0f, 5.0f);
        t = 0;

        int rnd = Random.Range(0,3);

        switch (rnd)
        {
            case 0:
                action = ActionType.Still;
                break;
            case 1:
                action = ActionType.Walk;
                break;
            case 2:
                action = ActionType.RotateLeft;
                actionTime /= 2;
                break;
            case 3:
                action = ActionType.RotateRight;
                actionTime /= 2;
                break;
            default:
                break;
        }
    }
}

public enum ActionType
{
    Still,
    Walk,
    RotateRight,
    RotateLeft
}
