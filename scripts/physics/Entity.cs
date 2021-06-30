using Godot;

namespace Bonebreaker.Physics
{
    public abstract class Entity : Node2D
    {
        ///<summary> The Entity real position, with decimal precision </summary>
        protected FixedVector2 RealPosition
        {
            get
            {
                return _realPosition;
            }
            set
            {
                _realPosition = value;
                UpdatePosition();
            }
        }

        private FixedVector2 _realPosition;


        /// <summary>
        /// Update the position of the Entity
        /// </summary>
        private void UpdatePosition ()
        {
            GlobalPosition = new Vector2( (int)_realPosition.X, (int)_realPosition.Y);
        }

        
    }
}