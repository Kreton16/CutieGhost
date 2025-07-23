using UnityEngine;

public class ButtonSound : MonoBehaviour
{

    public void PlaySound(string soundName)
    {
        SoundManager.Instance.PlaySound2D(soundName);
    }
}