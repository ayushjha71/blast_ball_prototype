using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace blastBall.handler
{
    public class Slot : MonoBehaviour
    {
        private bool isFilled = false;
        private Color? mCurrentColor = null;
        private int mRequiredCount = 0;
        private ColorCounter mCounter;
        private SpriteRenderer mSpriteRenderer;

        private void Awake()
        {
            mSpriteRenderer = GetComponent<SpriteRenderer>();
            if (mSpriteRenderer == null)
            {
                mSpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        public void FillSlot(Color color, int count)
        {
            mCurrentColor = color;
            isFilled = true;
            mRequiredCount = count;

            // Set the slot color with full opacity
            if (mSpriteRenderer != null)
            {
                mSpriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
            }

            if (mCounter == null)
            {
                ColorCounter counterPrefab = FindObjectOfType<ColorDisplay>().CounterPrefab;
                if (counterPrefab != null)
                {
                    mCounter = Instantiate(counterPrefab, transform.position + new Vector3(0.3f, 0.3f, 0), Quaternion.identity, transform);
                }
            }

            if (mCounter != null)
            {
                mCounter.SetColor(color, count);
            }
        }

        public void Clear()
        {
            isFilled = false;
            mCurrentColor = null;
            mRequiredCount = 0;

            if (mCounter != null)
            {
                Destroy(mCounter.gameObject);
                mCounter = null;
            }

            if (mSpriteRenderer != null)
            {
                // Make the slot semi-transparent white when empty
                mSpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            }
        }

        public void UpdateCount(int newCount)
        {
            mRequiredCount = newCount;
            if (mCounter != null)
            {
                mCounter.UpdateCount(newCount);
            }
        }

        public bool IsFilled() => isFilled;
        public Color? GetColor() => mCurrentColor;
        public int GetRequiredCount() => mRequiredCount;
    }
}
