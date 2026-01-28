using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public RoomShrinkController room;

    // Bu fonksiyonu sen kapı kapanınca çağıracaksın
    public void OnDoorClosed()
    {
        room.StartShrinking();
    }
}
