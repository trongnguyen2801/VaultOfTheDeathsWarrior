using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Input : MonoBehaviour
{
    public float verticalInput;
    public float horizontalInput;
    
    public bool mouseButtonDown;
    public bool spaceKeyDown;
    public bool shirftKeyDown;

    void Update()
    {
        if (!mouseButtonDown && Time.timeScale != 0)
        {
            mouseButtonDown = Input.GetMouseButtonDown(0);
        }

        if (!spaceKeyDown && Time.timeScale != 0)
        {
            spaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        }
        if (!shirftKeyDown && Time.timeScale != 0)
        {
            shirftKeyDown = Input.GetKeyDown(KeyCode.LeftShift);
        }
        
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void OnDisable()
    {
        ClearCache();
    }

    public void ClearCache()
    {
        verticalInput = 0;
        horizontalInput = 0;
        spaceKeyDown = false;
        shirftKeyDown = false;
        mouseButtonDown = false;
    }
}
