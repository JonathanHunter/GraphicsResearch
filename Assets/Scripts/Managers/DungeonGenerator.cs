namespace GraphicsResearch.Managers
{
    using System.Collections;
    using UnityEngine;
    using Generation;
    using Generation.Rooms;

    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField]
        private Floor floorPrefab;

        [SerializeField]
        private RoomGenerator roomGenerator;

        [SerializeField]
        private Transform floorStart;

        [SerializeField]
        private bool placeRoomsOnStart = false;

        [SerializeField]
        private int numFloors;

        private Floor[] floors;

        private void Start()
        {
            floors = new Floor[this.numFloors];
            for (int i = 0; i < this.numFloors; i++)
            {
                Floor f = Instantiate(this.floorPrefab);
                f.transform.position = new Vector3(this.floorStart.position.x, this.floorStart.position.y - 20 * i, this.floorStart.position.z);
                f.transform.rotation = this.floorStart.rotation;
                f.transform.localScale = this.floorStart.localScale;
                floors[i] = f;
            }

            StartCoroutine(StartUpAsync());
        }

        private void Update()
        {
        }

        private IEnumerator StartUpAsync()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            if (this.placeRoomsOnStart)
            {
                stopwatch.Start();
                for (int i = 0; i < this.floors.Length; i++)
                    yield return StartCoroutine(this.roomGenerator.PlaceRoomsAsync(floors[i]));
                stopwatch.Stop();
                Debug.Log("finished placing rooms in " + stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }
    }
}
