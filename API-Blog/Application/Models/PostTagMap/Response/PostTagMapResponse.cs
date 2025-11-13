namespace Application.DTOs.PostTagMap
{
    public class PostTagMapResponse
    {
        public int PostId { get; set; }
        public List<string> TagNames { get; set; } = new List<string>();
    }
}
