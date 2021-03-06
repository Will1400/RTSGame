﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public UnityEvent Escape;
    public UnityEvent Cancel;

    public UnityEvent OrderMove;
    public UnityEvent OrderAttack;
    public UnityEvent OrderStop;

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);

        Escape = new UnityEvent();
        Cancel = new UnityEvent();

        // Orders
        OrderMove = new UnityEvent();
        OrderAttack = new UnityEvent();
        OrderStop = new UnityEvent();
    }

    // Use this for initialization
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.CursorState == CursorState.None)
        {
            CheckOrders();
        }
        if (gameManager.CursorState == CursorState.Selecting || gameManager.CursorState == CursorState.Building)
        {
            CheckCancel();
        }
    }

    void CheckOrders()
    {
        if (Input.GetButton("Move") || Input.GetButton("Secondary Mouse"))
        {
            OrderMove.Invoke();
        }
        if (Input.GetButton("Attack"))
        {
            OrderAttack.Invoke();
        }
        if (Input.GetButton("Stop"))
        {
            GiveOrderStop();
        }
    }

    void CheckCancel()
    {
        if (Input.GetButton("Escape") || Input.GetButton("Cancel") || Input.GetButton("Secondary Mouse"))
        {
            Cancel.Invoke();
        }
    }

    public void GiveOrderStop()
    {
        OrderStop.Invoke();
    }
}
