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
        private bool async = false;

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

            if (this.async)
                StartCoroutine(StartUpAsync());
            else
                StartUp();
        }

        private void Update()
        {
            if (this.disableInput)
                return;

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_Circle))
                this.roomManager[0].PlaceRoom(true);

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_Rect))
                this.roomManager[0].PlaceRoom(false);

            if (CustomInput.BoolHeld(CustomInput.UserInput.DartThrow_Repulse))
                this.roomManager[0].Repulse();
            else if (CustomInput.BoolUp(CustomInput.UserInput.DartThrow_Repulse))
                this.roomManager[0].Halt();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_Jittered))
                this.roomManager[0].PlaceRooms();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_ShowObjects))
                this.roomManager[0].ToggleRooms();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Vst_Calculate))
                this.pathManager[0].PlacePaths(this.roomManager[0]);

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Vst_Randomized))
                this.pathManager[0].RandomizeExtraEdges();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Vst_Ordered))
                this.pathManager[0].SortExtraEdges();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Rasterize_Rasterize))
                this.meshManager[0].CalculateMesh(this.roomManager[0], this.pathManager[0]);

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Rasterize_GenMesh))
                this.meshManager[0].CreateMesh();
        }

        private void StartUp()
        {
            for (int i = 0; i < this.meshManager.Length; i++)
            {
                if (this.placeRoomsOnStart)
                {
                    this.roomManager[i].PlaceRooms();
                }

                if (this.placePathsOnStart)
                {
                    this.pathManager[i].PlacePaths(this.roomManager[i]);
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
                }
            }

            for (int i = 0; i < this.meshManager.Length; i++)
            {
                foreach (Room r in this.roomManager[i].Rooms)
                    r.GetComponent<Collider>().enabled = true;
            }

            for (int i = 0; i < this.meshManager.Length - 1; i++)
            {
                this.multiFloorManager.FindRoomPairs(this.roomManager[i], this.roomManager[i + 1]);
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
                    this.meshManager[i].GenerateMesh(this.roomManager[i], this.pathManager[i]);
                }
            }

            if (placeMeshOnStart)
            {
                this.player.transform.parent = this.meshManager[0].gameObject.transform;
                this.player.transform.localPosition = roomManager[0].Rooms[0].transform.position;
                this.player.transform.parent = null;
                this.player.transform.rotation = Quaternion.identity;
                this.player.transform.localScale = Vector3.one;
            }
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
                yield return StartCoroutine(this.multiFloorManager.FindRoomPairsAsync(this.roomManager[i], this.roomManager[i + 1]));
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
