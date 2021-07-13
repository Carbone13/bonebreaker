namespace Bonebreaker.Inputs
{
    public interface InputHandler
    {
        int ID { get; set; }

        public InputState GetState ();
    }
}