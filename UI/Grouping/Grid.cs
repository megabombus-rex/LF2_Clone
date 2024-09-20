using LF2Clone.Base;
using Raylib_cs;
using System.Numerics;
using System.Xml.Linq;

namespace LF2Clone.UI.Grouping
{
    // For UI
    public class Grid : UIGrouper
    {
        public List<List<Node?>> _matrix { get; private set; }
        private int _columnCount;
        private int _rowCount;
        private int _columnSize;
        private int _rowSize;

        public Grid(int columnCount, int rowCount, float rotation, Transform transform, bool isActive, string name, int id) 
            : base(rotation, transform, isActive, name, id) 
        { 
            _matrix = new List<List<Node?>>();
            _columnCount = columnCount;
            _rowCount = rowCount;
            

            _matrix.EnsureCapacity(columnCount);
            foreach (List<Node> row in _matrix)
            {
                row.EnsureCapacity(rowCount);
            }
        }

        public void AssignNodeToPosition(int posX, int posY, Node node)
        {
            if (posX > _columnCount || posX < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Selected column is invalid. PosX: {0} Node: {1}", posX, node._name));
            }

            if (posY > _rowCount || posY < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Selected row is invalid. PosY: {0} Node: {1}", posY, node._name));
            }

            _matrix[posX][posY] = node;
        }

        public void ClearPosition(int posX, int posY)
        {
            if (posX > _columnCount || posX < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Selected column is invalid. PosX: {0}", posX));
            }

            if (posY > _rowCount || posY < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Selected row is invalid. PosY: {0}", posY));
            }

            _matrix[posX][posY] = null;
        }

        public override void Destroy()
        {
            _matrix.Clear();
            base.Destroy();
        }

        // event OnGridChanged
    }
}
