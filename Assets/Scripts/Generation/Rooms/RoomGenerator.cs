namespace GraphicsResearch.Generation.Rooms
{
    using System.Collections;
    using UnityEngine;

    public abstract class RoomGenerator : MonoBehaviour
    {
        /// <summary> Initializes this room manager. </summary>
        public void Init()
        {
            LocalInit();
        }

        /// <summary> Clears any previous rooms and places a new set of rooms asyncronously. </summary>
        public IEnumerator PlaceRoomsAsync(Floor f)
        {
            yield return StartCoroutine(LocalPlaceRoomsAsync(f));
        }

        /// <summary> Local handler for initialization. </summary>
        protected abstract void LocalInit();
        /// <summary> Local async handler for room placement. </summary>
        protected abstract IEnumerator LocalPlaceRoomsAsync(Floor f);
    }
}
