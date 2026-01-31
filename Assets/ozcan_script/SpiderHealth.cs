using UnityEngine;

public class SpiderHealth : MonoBehaviour
{
    public int hitsToDie = 2;
    public float destroyDelay = 0f;

    private int _hits;

    // öldü mü? bilgisini döndürür
    public bool TakeHit()
    {
        _hits++;
        if (_hits >= hitsToDie)
        {
            Destroy(gameObject, destroyDelay);
            return true;
        }
        return false;
    }
}
