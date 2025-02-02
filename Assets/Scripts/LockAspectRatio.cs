using UnityEngine;

public class LockAspectRatio : MonoBehaviour
{
    void Awake()
    {
        LockAspect();
    }

    void LockAspect()
    {
        // Set the desired aspect ratio (16:9)
        float targetAspect = 16f / 9f;

        // Get the current screen's aspect ratio
        float currentAspect = (float)Screen.width / (float)Screen.height;

        // If the current aspect ratio is different from 16:9, adjust the game view
        if (currentAspect != targetAspect)
        {
            // Calculate the scaling factor to fit the screen
            float scaleHeight = currentAspect / targetAspect;
            if (scaleHeight < 1f)
            {
                // Add black bars on top/bottom
                Camera.main.rect = new Rect(0, (1f - scaleHeight) / 2f, 1f, scaleHeight);
            }
            else
            {
                // Add black bars on left/right
                float scaleWidth = 1f / scaleHeight;
                Camera.main.rect = new Rect((1f - scaleWidth) / 2f, 0, scaleWidth, 1f);
            }
        }
    }
}
