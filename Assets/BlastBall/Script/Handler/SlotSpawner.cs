using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSpawner : MonoBehaviour
{
    [SerializeField]
    float slotSpacing = 2;
    public Slot slotPrefab;
    private Slot[] slots;
    public float slotsVerticalOffset = -3f;

    public void SetupSlots(int numSlots)
    {
        slots = new Slot[numSlots];

        Camera mainCamera = Camera.main;
        float screenWidthWorldUnits = mainCamera.orthographicSize * 2 * mainCamera.aspect;
        float totalWidth = (numSlots - 1) * slotSpacing;
        float startX = -screenWidthWorldUnits / 2 + (screenWidthWorldUnits - totalWidth) / 2;

        Vector3 slotStartPosition = new Vector3(startX, slotsVerticalOffset, 0);

        for (int i = 0; i < numSlots; i++)
        {
            Slot newSlot = Instantiate(slotPrefab, slotStartPosition + new Vector3(i * slotSpacing, 0, 0), Quaternion.identity);
            slots[i] = newSlot;
        }
    }
}
