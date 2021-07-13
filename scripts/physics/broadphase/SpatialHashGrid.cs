using System;
using System.Collections.Generic;
using Godot;

namespace Bonebreaker.Physics.Broadphase
{
    public class GridObject
    {
        public int ID;
        public int2 Min, Max;
    }
    
    public class SpatialHashGrid<T> where T : class, IGridElement
    {
         /// <summary>
        /// the size of the grid
        /// </summary>
        readonly int _width, _height;

        /// <summary>Stores all the rows in the grid</summary>
        private UGridRow[] rows;

        // Stores the number of columns and rows in the grid.
        readonly int _numRows, _numCols;

        /// <summary>
        /// stores the size of a cell
        /// </summary>
        readonly float _invCellWidth, _invCellHeight;
        
        
        // Returns a new grid storing elements that have a uniform upper-bound size. Because 
        // all elements are treated uniformly-sized for the sake of search queries, each one 
        // can be stored as a single point in the grid.
        public SpatialHashGrid(float cWidth, float cHeight,
            int gridWidth, int gridHeight)
        {
            _numRows = (int)(gridHeight / cHeight) + 1;
            _numCols = (int)(gridWidth / cWidth) + 1;

            _invCellWidth = 1 / cWidth;
            _invCellHeight = 1 / cHeight;
            _width = gridWidth;
            _height = gridHeight;
            
            rows = new UGridRow[_numRows];
            for (int i = 0; i < _numRows; i++)
            {
                rows[i] = new UGridRow();
                rows[i].cells = new T[_numCols];
            }
        }
        
        public void Insert(T obj)
        {
            int minX = GridLocalToCellCol(obj.Min.X);
            int minY = GridLocalToCellRow(obj.Min.Y);
            int maxX = GridLocalToCellCol(obj.Max.X);
            int maxY = GridLocalToCellRow(obj.Max.Y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    InsertToCell(obj, x, y);
                }
            }
        }

        private void InsertToCell(T obj, int xIdx, int yIdx)
        {
            UGridRow row = rows[yIdx];
            obj.NextElement[new int2(xIdx, yIdx)] = row.cells[xIdx];

            row.cells[xIdx] = obj;
            ++row.eltCount;
        }

        /// <summary>
        /// Removes an element from the grid. The <see cref="IUGridElt"/> of the object must give
        /// the same position where the element is currently in the grid
        /// </summary>
        /// <param name="obj">The object to remove</param>
        public void Remove(T obj)
        {
            int minX = GridLocalToCellCol(obj.Min.X);
            int minY = GridLocalToCellRow(obj.Min.Y);
            int maxX = GridLocalToCellCol(obj.Max.X);
            int maxY = GridLocalToCellRow(obj.Max.Y);
            
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    RemoveFromCell(obj, x, y);
                }
            }
        }

        private void RemoveFromCell(T obj, int xIdx, int yIdx)
        {
            UGridRow row = rows[yIdx];

            T elt = row.cells[xIdx];
            T prevElt = null;

            while (elt.ID != obj.ID)
            {
                prevElt = elt;
                elt = (T)elt.NextElement[new int2(xIdx, yIdx)];

                if (elt == null)
                {
                    throw new Exception("Element not found");
                }
            }

            if (prevElt == null)
                row.cells[xIdx] = (T)elt.NextElement[new int2(xIdx, yIdx)];
            else
                prevElt.NextElement[new int2(xIdx, yIdx)] = elt.NextElement[new int2(xIdx, yIdx)];

            --row.eltCount;
        }

        /// <summary>
        /// Moves an element in the grid from the former position to the new one.
        /// </summary>
        /// <param name="obj">The object to move</param>
        public void Move(T obj, int2 oldMin, int2 oldMax)
        {
            int oldMinX = GridLocalToCellCol(oldMin.X);
            int oldMinY = GridLocalToCellRow(oldMin.Y);
            int oldMaxX = GridLocalToCellCol(oldMax.X);
            int oldMaxY = GridLocalToCellRow(oldMax.Y);
            
            int newMinX = GridLocalToCellCol(obj.Min.X);
            int newMinY = GridLocalToCellRow(obj.Min.Y);
            int newMaxX = GridLocalToCellCol(obj.Max.X);
            int newMaxY = GridLocalToCellRow(obj.Max.Y);

            // Min did not change
            if (oldMinX == newMinX && oldMinY == newMinY)
            {
                // Max did not change
                if (oldMaxX == newMaxX && oldMaxY == newMaxY)
                {
                    return;
                }
            }
            
            for (int y = oldMinY; y <= oldMaxY; y++)
            {
                for (int x = oldMinX; x <= oldMaxX; x++)
                {
                    RemoveFromCell(obj, x, y);
                }
            }
            
            for (int y = newMinY; y <= newMaxY; y++)
            {
                for (int x = newMinX; x <= newMaxX; x++)
                {
                    InsertToCell(obj, x, y);
                }
            }
        }

        private readonly List<T> _queryResults = new List<T>(16);
        // Returns all the element IDs that intersect the specified rectangle excluding 
        // elements with the specified ID to omit.
        public List<T> Query(AABB range, int omitEltID = int.MinValue)
        {
            int minX = GridLocalToCellCol(range.Min.X);
            int minY = GridLocalToCellRow(range.Min.Y);
            int maxX = GridLocalToCellCol(range.Max.X);
            int maxY = GridLocalToCellRow(range.Max.Y);
            
            _queryResults.Clear();
            
            for (int y = minY; y <= maxY; y++)
            {
                UGridRow row = rows[y];
                if (row.eltCount == 0)
                    continue;

                for (int x = minX; x <= maxX; x++)
                {
                    T elt = row.cells[x];
                    while (elt != null)
                    {
                        if (PointInRect(elt, in range) && elt.ID != omitEltID)
                            _queryResults.Add(elt);
                        elt = (T)elt.NextElement[new int2(x, y)];
                    }
                }
            }
            
            return _queryResults;
        }

        /// <summary>
        /// Calls <see cref="IUniformGridVisitor.Grid(float, float, float, float)"/> with the grid data
        /// Then traverses the whole grid and calls <see cref="IUniformGridVisitor.Cell(IUGridElt, int, int, float, float)"/>
        /// on any non-empty cells.
        /// </summary>
        public void Traverse(GridDrawer visitor)
        {
            visitor.DrawGrid(_width, _height, 1f / _invCellWidth, 1f / _invCellHeight);

            for (int y = 0; y < _numRows; y++)
            {
                UGridRow row = rows[y];
                if (row.eltCount == 0)
                    continue;

                for (int x = 0; x < _numCols; x++)
                {
                    if (row.cells[x] != null)
                    {
                        visitor.DrawCell(row.cells[x], x, y, 1f / _invCellWidth, 1f / _invCellHeight);
                    }
                }
            }
        }

        #region Private methods
        /// <summary>
        /// Returns the grid cell Y index for the specified position
        /// </summary>
        private int GridLocalToCellRow(float y)
        {
            if (y <= 0) { return 0; }
            return Math.Min((int)(y * _invCellHeight), _numRows - 1);
        }

        /// <summary>
        /// Returns the grid cell X index for the specified position
        /// </summary>
        private int GridLocalToCellCol(float x)
        {
            if (x <= 0) { return 0; }
            return Math.Min((int)(x * _invCellWidth), _numCols - 1);
        }

        private static bool PointInRect(in IGridElement elt, in AABB rect)
        {
            if (elt.Max.X < rect.Min.X || elt.Min.X > rect.Max.X) return false;
            if (elt.Max.Y < rect.Min.Y || elt.Min.Y > rect.Max.Y) return false;

            return true;
        }
        #endregion

        /// <summary>
        /// Just an array of type T and a cached count of the elements in the row
        /// </summary>
        class UGridRow
        {
            // Stores all the cells in the row. 
            // Each cell stores the first element in that cell, 
            // which points to the next in the elts list.
            public T[] cells;

            // Stores the number of elements in the row.
            public int eltCount;
        }
    }
}