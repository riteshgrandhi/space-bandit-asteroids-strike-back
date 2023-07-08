using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchController : MonoBehaviour
{
    public Enemy[] launchableEnemies;

    public SpriteRenderer spriteRenderer;

    LauncherState state = LauncherState.PLACING;

    int currentSelection = 0;
    private Vector3 aimPosition;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = launchableEnemies[currentSelection].GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateAimPosition();

        switch (state)
        {
            case LauncherState.PLACING:
                UpdatePlacementAndSelection();
                break;
            case LauncherState.AIMING:
                UpdateAim();
                break;
        }
    }

    private void CalculateAimPosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimPosition = new Vector3(mousePos.x, mousePos.y, 0);
    }

    void UpdatePlacementAndSelection()
    {

        if (Input.GetMouseButtonUp(0))
        {
            state = LauncherState.AIMING;
            return;
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) // forward
        {
            int delta = Mathf.FloorToInt(Input.GetAxis("Mouse ScrollWheel") * 10);
            currentSelection = (currentSelection + delta) % launchableEnemies.Length;
            if (currentSelection < 0)
            {
                currentSelection = launchableEnemies.Length + currentSelection;
            }
            spriteRenderer.sprite = launchableEnemies[currentSelection].GetComponent<SpriteRenderer>().sprite;
        }

        spriteRenderer.transform.position = aimPosition;
    }

    private void UpdateAim()
    {

        Vector3 sourcePos = spriteRenderer.transform.position;
        Debug.DrawLine(sourcePos, aimPosition);

        if (Input.GetMouseButtonUp(0))
        {
            Enemy launchedEnemy = Instantiate(launchableEnemies[currentSelection]);
            launchedEnemy.transform.position = sourcePos;
            Vector2 aimVector = aimPosition - sourcePos;
            launchedEnemy.SetForceAndDirection(aimVector.magnitude, aimVector.normalized);

            state = LauncherState.PLACING;
            return;
        }
    }
}

enum LauncherState
{
    PLACING,
    AIMING
}