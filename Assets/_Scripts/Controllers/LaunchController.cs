using System;
using UnityEngine;

public class LaunchController : MonoBehaviour
{
    public Enemy[] launchableEnemies;

    public SpriteRenderer spriteRenderer;
    public Transform aimArrow;
    LauncherState state = LauncherState.PLACING;
    int currentSelection = 0;
    Vector3 mousePosition;
    int placementNotAllowedLayerMask;

    void Awake()
    {
        spriteRenderer.sprite = launchableEnemies[currentSelection].GetComponent<SpriteRenderer>().sprite;

        placementNotAllowedLayerMask = LayerMask.GetMask("PlacementNotAllowed", "Bounds");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMousePosition();

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

    private void CalculateMousePosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePos.x, mousePos.y, 0);
    }

    void UpdatePlacementAndSelection()
    {
        bool isPlacementAllowed = IsPlacementAllowed(out RaycastHit2D raycastHit);

        if (Input.GetMouseButtonUp(0))
        {
            if (isPlacementAllowed)
            {
                state = LauncherState.AIMING;
                aimArrow.GetComponentInChildren<SpriteRenderer>().enabled = true;
                return;
            }
            else
            {
                raycastHit.collider.gameObject.GetComponent<BlinkController>()?.StartBlink(4);
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            int delta = Mathf.FloorToInt(Input.GetAxis("Mouse ScrollWheel") * 10);
            currentSelection = (currentSelection + delta) % launchableEnemies.Length;
            if (currentSelection < 0)
            {
                currentSelection = launchableEnemies.Length + currentSelection;
            }
            spriteRenderer.sprite = launchableEnemies[currentSelection].GetComponent<SpriteRenderer>().sprite;
        }

        spriteRenderer.transform.position = mousePosition;
    }

    private bool IsPlacementAllowed(out RaycastHit2D raycastHit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        raycastHit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, placementNotAllowedLayerMask);
        if (raycastHit)
        {
            return false;
        }

        return true;
    }

    private void UpdateAim()
    {
        Vector3 sourcePos = spriteRenderer.transform.position;
        Vector2 aimVector = mousePosition - sourcePos;
        aimArrow.position = sourcePos;
        float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
        aimArrow.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Debug.DrawLine(sourcePos, mousePosition, aimVector.magnitude <= 4 || aimVector.magnitude >= 10 ? Color.red : Color.green);

        if (Input.GetMouseButtonUp(0))
        {
            Enemy launchedEnemy = Instantiate(launchableEnemies[currentSelection]);
            launchedEnemy.transform.position = sourcePos;
            launchedEnemy.SetForceAndDirection(aimVector.magnitude, aimVector.normalized);

            state = LauncherState.PLACING;
            aimArrow.GetComponentInChildren<SpriteRenderer>().enabled = false;
            return;
        }
    }
}

enum LauncherState
{
    PLACING,
    AIMING
}