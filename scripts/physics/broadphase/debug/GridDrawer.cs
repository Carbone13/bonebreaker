using Godot;

namespace Bonebreaker.Physics.Broadphase
{
    public class GridDrawer : Node2D
    {
        public SpatialHashGrid<Pushbox> target;

        public override void _Process (float delta)
        {
            Update();
        }

        public override void _Draw ()
        {
            target?.Traverse(this);
        }

        public void DrawGrid (float width, float height, float cellWidth, float cellHeight)
        {
            float numRows = (height / cellHeight) + 1;
            float numCols = (width / cellWidth) + 1;
            width = cellWidth * numCols;
            height = cellHeight * numRows;
            
            DrawRect(new Rect2(new Vector2(width / 2f, height / 2f), new Vector2(width, height)), Colors.Blue, false);

            for (int i = 1; i < numRows; i++)
            {
                DrawLine(new Vector2(0, i * cellHeight), new Vector2(width, i * cellHeight), Colors.Blue);
            }
            for (int i = 1; i < numCols; i++)
            {
                DrawLine(new Vector2(i * cellWidth, 0), new Vector2(i * cellWidth, height), Colors.Blue);
            }
        }

        public void DrawCell(IGridElement firstElt, int x, int y, float cellWidth, float cellHeight)
        {
            float halfWidth = cellWidth / 2;
            float halfHeight = cellHeight / 2;

            Color col = new Color(0, 0, 1, 0.3f);

            DrawRect(new Rect2(new Vector2((x * cellWidth), (y * cellHeight)),
                new Vector2(cellWidth, cellHeight)), col);
        }
    }

}