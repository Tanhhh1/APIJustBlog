namespace Application.DTOs.PostTagMap
{
    public class PostTagMapSaveDTO
    {
        public int PostId { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
