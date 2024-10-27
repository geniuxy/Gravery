using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int soundIndex;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.instance.PlaySFX(soundIndex);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.instance.StopSFXWithDelay(soundIndex, .25f);
    }
}