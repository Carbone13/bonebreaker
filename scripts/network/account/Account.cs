using LiteNetLib.Utils;

namespace Bonebreaker.Net
{
    public class Account : INetSerializable
    {
        public int ID = -1;
        public string Username, Email, Password;
        
        public void Serialize (NetDataWriter writer)
        {
            writer.Put(ID);
            writer.Put(Username, 50);
            writer.Put(Email, 100);
            writer.Put(Password, 100);
        }

        public void Deserialize (NetDataReader reader)
        {
            ID = reader.GetInt();
            Username = reader.GetString(50);
            Email = reader.GetString(100);
            Password = reader.GetString(100);
        }

        public override string ToString ()
        {
            return "id: " + ID + " username: " + Username + " pwd: " + Password + " email: " + Email;
        }
    }
}