using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RazvanRumble : MonoBehaviour
{
    [Header("Haptics Settings")]
    public float hapticIntensity = 0.5f;     // Range: 0.0 to 1.0
    public float hapticDuration = 0.1f;      // Duration in seconds
    public float hapticCooldown = 0.2f;      // Time between haptics

    private float nextHapticTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (Time.time >= nextHapticTime)
        {
            XRBaseController controller = other.GetComponentInParent<XRBaseController>();
            if (controller != null)
            {
                controller.SendHapticImpulse(hapticIntensity, hapticDuration);
                nextHapticTime = Time.time + hapticCooldown;
            }
        }
    }
}
