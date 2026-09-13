namespace HMS.Core.Entiites.RoomModuleEntities
{
    public class RoomImage:BaseEntity<int>
    {
        public string ImageUrl { get; set; } = null!;
        public int RoomId { get; set; }
    }
}