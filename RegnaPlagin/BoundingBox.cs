using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renga;
using static System.Net.Mime.MediaTypeNames;

namespace RegnaPlagin
{
    public class BoundingBox
    {
        private readonly IApplication _app;
        private readonly IUI _uiApp;

        public BoundingBox(IApplication app, IUI ui)
        {
            _app = app;
            _uiApp = ui;
        }

        public void ActinHandler(object sender, EventArgs e)
        {
            float minX = 0, minY = 0, minZ = 0, maxX = 0, maxY = 0, maxZ = 0;
            List<IExportedObject3D> listObj3dWall = GetObject3dByType(ObjectTypes.Wall);
            List<IGrid> listGrid = GetGridsByObject3D(listObj3dWall);

            // Первоначальные значения
            if (listGrid.Count > 0)
            {
                if (listGrid[0].VertexCount > 0)
                {
                    listGrid[0].GetVertexComponents(0, out minX, out minY, out minZ);
                    maxX = minX; maxY = minY; maxZ = minZ;
                }
            }

            // Перебор вертексов в сетке объектов min/max
            foreach (IGrid grid in listGrid)
            {
                for (int i = 0; i < grid.VertexCount; i++)
                {
                    grid.GetVertexComponents(i, out float x, out float y, out float z);
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                    if (z < minZ) minZ = z;
                    if (z > maxZ) maxZ = z;
                }

            }

            _uiApp.ShowMessageBox(MessageIcon.MessageIcon_Info, "Ограничивающий объем стен проекта"
                , $"Минимальная точка X:{minX} Y:{minY} Z:{minZ}\nМаксимальная точка X:{maxX} Y:{maxY} Z:{maxZ}"); 
        }

        private List<IExportedObject3D> GetObject3dByType(Guid type)
        {
            IProject project = _app.Project;
            IDataExporter dataExporter = project.DataExporter;
            IExportedObject3DCollection objectCollection = dataExporter.GetObjects3D();

            List<IExportedObject3D> listObj3D = new List<IExportedObject3D>();
            for (int objectIndex = 0; objectIndex < objectCollection.Count; ++objectIndex)
            {
                IExportedObject3D object3d = objectCollection.Get(objectIndex);
                if (object3d.ModelObjectType == type) listObj3D.Add(object3d);
            }
            return listObj3D;
        }

        private List<IGrid> GetGridsByObject3D(List<IExportedObject3D> listObj3D)
        {
            List<IGrid> grids = new List<IGrid>();

            foreach (var object3D in listObj3D)
            {
                for (int meshIndex = 0; meshIndex < object3D.MeshCount; ++meshIndex)
                {
                    IMesh mesh = object3D.GetMesh(meshIndex);
                    for (int gridIndex = 0; gridIndex < mesh.GridCount; ++gridIndex)
                    {
                        IGrid grid = mesh.GetGrid(gridIndex);
                        grids.Add(grid);
                    }
                }
            }
            return grids;
        }
    }
}
