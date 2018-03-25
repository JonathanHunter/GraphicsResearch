namespace GraphicsResearch.Managers
{
    using System.Collections;
    using UnityEngine;
    using MeshGeneration;
    using MultiFloorGeneration;
    using PathPlacement;
    using RoomPlacement;
    using Util;

    public class GenerationManager : MonoBehaviour
    {
        [SerializeField]
        private bool placeRoomsOnStart = false;
        [SerializeField]
        private bool placePathsOnStart = false;
        [SerializeField]
        private bool placeMeshOnStart = false;
        [SerializeField]
        private bool toggleRoomsOnStart = false;

        [SerializeField]
        private GameObject player;
        [SerializeField]
        private bool disableInput;

        public RoomManager[] roomManager;
        public PathManager[] pathManager;
        public MeshManager[] meshManager;
        public MultiFloorManager multiFloorManager;

        private void Start()
        {
            foreach (RoomManager r in this.roomManager)
                r.Init();
            foreach (PathManager p in this.pathManager)
                p.Init();
            foreach (MeshManager m in this.meshManager)
                m.Init();

            this.multiFloorManager.Init();

            StartCoroutine(StartUpAsync());
        }

        private void Update()
        {
        }
        
        private IEnumerator StartUpAsync()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            for (int i = 0; i < this.meshManager.Length; i++)
            {
                if (this.placeRoomsOnStart)
                {
                    stopwatch.Start();
                    yield return StartCoroutine(this.roomManager[i].PlaceRoomsAsync());
                    stopwatch.Stop();
                    Debug.Log("finished placing rooms in " + stopwatch.Elapsed);
                    stopwatch.Reset();
                }

                if (this.placePathsOnStart)
                {
                    stopwatch.Start();
                    yield return StartCoroutine(this.pathManager[i].PlacePathsAsync(this.roomManager[i]));
                    stopwatch.Stop();
                    Debug.Log("finished placing paths in " + stopwatch.Elapsed);
                    stopwatch.Reset();
                }

                foreach (Room r in this.roomManager[i].Rooms)
                {
                    r.GetComponent<Collider>().enabled = false;
                    Vector3 pos = r.transform.position;
                    Vector3 size = r.transform.localScale;
                    Quaternion rot = r.transform.rotation;
                    r.transform.parent = this.meshManager[i].gameObject.transform;
                    r.transform.localPosition = pos;
                    r.transform.localScale = size;
                    r.transform.localRotation = rot;
                    r.transform.parent = null;
                }

                if (this.toggleRoomsOnStart)
                {
                    this.roomManager[i].ToggleRooms();
                    yield return null;
                }
            }

            for (int i = 0; i < this.meshManager.Length; i++)
            {
                foreach (Room r in this.roomManager[i].Rooms)
                    r.GetComponent<Collider>().enabled = true;
            }

            for (int i = 0; i < this.meshManager.Length - 1; i++)
            {
                stopwatch.Start();
                yield return StartCoroutine(this.multiFloorManager.FindRoomPairsAsync(this.roomManager[i], this.roomManager[i + 1], this.pathManager[i], this.pathManager[i + 1]));
                this.multiFloorManager.RasterizeHallways(this.meshManager[i], this.meshManager[i + 1]);
                stopwatch.Stop();
                Debug.Log("finished placing multifloor paths in " + stopwatch.Elapsed);
                stopwatch.Reset();
            }

            for (int i = 0; i < this.meshManager.Length; i++)
            {
                foreach (Room r in this.roomManager[i].Rooms)
                    r.GetComponent<Collider>().enabled = false;
            }

            if (this.placeMeshOnStart)
            {
                for (int i = 0; i < this.meshManager.Length; i++)
                {
                    stopwatch.Start();
                    yield return StartCoroutine(this.meshManager[i].GenerateMeshAsync(this.roomManager[i], this.pathManager[i]));
                    stopwatch.Stop();
                    Debug.Log("finished generating mesh in " + stopwatch.Elapsed);
                    stopwatch.Reset();

                    Debug.Log("finished generation " + i);
                }
            }


            if (placeMeshOnStart)
            {
                this.player.transform.position = roomManager[0].Rooms[0].transform.position;
                this.player.transform.rotation = Quaternion.identity;
                this.player.transform.localScale = Vector3.one;
            }
        }
    }
}
