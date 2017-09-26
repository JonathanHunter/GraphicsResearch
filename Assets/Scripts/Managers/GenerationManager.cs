namespace GraphicsResearch.Managers
{
    using UnityEngine;
    using MeshGeneration;
    using PathPlacement;
    using RoomPlacement;
    using Util;

    public class GenerationManager : MonoBehaviour
    {
        public RoomManager roomManager;
        public PathManager pathManager;
        public MeshManager meshManager;

        private void Start()
        {
            this.roomManager.Init();
            this.pathManager.Init();
            this.meshManager.Init();
        }
        
        private void Update()
        {
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_Circle))
                this.roomManager.PlaceRoom(true);

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_Rect))
                this.roomManager.PlaceRoom(false);

            if (CustomInput.BoolHeld(CustomInput.UserInput.DartThrow_Repulse))
                this.roomManager.Repulse();
            else if (CustomInput.BoolUp(CustomInput.UserInput.DartThrow_Repulse))
                this.roomManager.Halt();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_Jittered))
                this.roomManager.PlaceRooms();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.DartThrow_ShowObjects))
                this.roomManager.ToggleRooms();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Vst_Calculate))
                this.pathManager.PlacePaths(this.roomManager);

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Vst_Randomized))
                this.pathManager.RandomizeExtraEdges();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Vst_Ordered))
                this.pathManager.SortExtraEdges();

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Rasterize_Rasterize))
                this.meshManager.CalculateMesh(this.roomManager, this.pathManager);

            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Rasterize_GenMesh))
                this.meshManager.CreateMesh();
        }
    }
}
