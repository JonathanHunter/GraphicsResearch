namespace GraphicsResearch.Managers
{
    using System.Collections;
    using UnityEngine;
    using Generation;
    using Generation.Meshes;
    using Generation.Paths;
    using Generation.Rooms;

    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField]
        private Floor floorPrefab;
        [SerializeField]
        private MiddleLayer layerPrefab;

        [SerializeField]
        private RoomGenerator roomGenerator;
        [SerializeField]
        private LayerPathGenerator layerPathGenerator;
        [SerializeField]
        private FloorPathGenerator floorPathGenerator;
        [SerializeField]
        private FloorRasterizer floorRasterizer;
        [SerializeField]
        private LayerRasterizer layerRasterizer;
        [SerializeField]
        private MeshGenerator meshGenerator;

        [SerializeField]
        private Transform floorStart;

        [SerializeField]
        private bool placeRoomsOnStart = false;
        [SerializeField]
        private bool placePathsOnStart = false;
        [SerializeField]
        private bool rasterizeOnStart = false;
        [SerializeField]
        private bool generateMeshOnStart = false;
        [SerializeField]
        private bool toggleRoomsOnStart = false;

        [SerializeField]
        private int numFloors;

        private Floor[] floors;
        private MiddleLayer[] layers;

        private void Start()
        {
            this.floors = new Floor[this.numFloors];
            this.layers = new MiddleLayer[this.numFloors - 1];
            for (int i = 0; i < this.numFloors; i++)
            {
                Floor f = Instantiate(this.floorPrefab);
                f.transform.position = new Vector3(this.floorStart.position.x, this.floorStart.position.y - 20 * i, this.floorStart.position.z);
                f.transform.rotation = this.floorStart.rotation;
                f.transform.localScale = this.floorStart.localScale;
                this.floors[i] = f;
            }
            
            for (int i = 0; i < this.numFloors - 1; i++)
            {
                MiddleLayer l = Instantiate(this.layerPrefab);
                l.transform.position = new Vector3(this.floorStart.position.x, this.floorStart.position.y - 20 * (i + 1), this.floorStart.position.z);
                l.transform.rotation = this.floorStart.rotation;
                l.transform.localScale = this.floorStart.localScale;
                this.layers[i] = l;
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
                    yield return StartCoroutine(this.roomGenerator.PlaceRoomsAsync(this.floors[i]));
                stopwatch.Stop();
                Debug.Log("finished placing rooms in " + stopwatch.Elapsed);
                stopwatch.Reset();

                if (this.placePathsOnStart)
                {
                    if (this.layers.Length > 0)
                    {
                        stopwatch.Start();
                        for (int i = 0; i < this.layers.Length; i++)
                            yield return StartCoroutine(this.layerPathGenerator.FindMiddleLayerPathsAsync(this.floors[i], this.floors[i + 1], this.layers[i]));
                        stopwatch.Stop();
                        Debug.Log("finished finding layer paths in " + stopwatch.Elapsed);
                        stopwatch.Reset();
                    }
                    
                    stopwatch.Start();
                    for (int i = 0; i < this.floors.Length; i++)
                    {
                        MiddleLayer up = null, down = null;
                        if(this.layers.Length > 0)
                        {
                            if (i > 0)
                                up = this.layers[i - 1];
                            if (i < this.floors.Length - 1)
                                down = this.layers[i];
                        }

                        yield return StartCoroutine(this.floorPathGenerator.FindFloorPathsAsync(this.floors[i], up, down));
                    }
                    stopwatch.Stop();
                    Debug.Log("finished finding floor paths in " + stopwatch.Elapsed);
                    stopwatch.Reset();
                }
                
                if (this.rasterizeOnStart)
                {
                    if (this.layers.Length > 0)
                    {
                        stopwatch.Start();
                        for (int i = 0; i < this.layers.Length; i++)
                            yield return StartCoroutine(this.layerRasterizer.RasterizeLayer(this.layers[i], this.floors[i], this.floors[i + 1]));
                        stopwatch.Stop();
                        Debug.Log("finished rasterizing layer paths in " + stopwatch.Elapsed);
                        stopwatch.Reset();
                    }

                    stopwatch.Start();
                    for (int i = 0; i < this.floors.Length; i++)
                        yield return StartCoroutine(this.floorRasterizer.RasterizeFloor(this.floors[i]));
                    stopwatch.Stop();
                    Debug.Log("finished rasterizing floors in " + stopwatch.Elapsed);
                    stopwatch.Reset();

                    if (this.generateMeshOnStart)
                    {
                        if (this.layers.Length > 0)
                        {
                            stopwatch.Start();
                            for (int i = 0; i < this.layers.Length; i++)
                                yield return StartCoroutine(this.meshGenerator.GenerateMesh(this.layers[i]));
                            stopwatch.Stop();
                            Debug.Log("finished generating layer mashes in " + stopwatch.Elapsed);
                            stopwatch.Reset();
                        }

                        stopwatch.Start();
                        for (int i = 0; i < this.floors.Length; i++)
                            yield return StartCoroutine(this.meshGenerator.GenerateMesh(this.floors[i]));
                        stopwatch.Stop();
                        Debug.Log("finished generating floor meshs in " + stopwatch.Elapsed);
                        stopwatch.Reset();
                    }
                }

                if (this.toggleRoomsOnStart)
                {
                    foreach(Floor f in this.floors)
                    {
                        foreach (Room r in f.Rooms)
                            r.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
