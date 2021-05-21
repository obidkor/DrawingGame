namespace ChatClientCS.Models
{
    public class User
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public byte[] Photo { get; set; }
        public int score { get; set; } = 0;
    }
}