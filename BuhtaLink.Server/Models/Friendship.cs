public class Friendship
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public string Status { get; set; } // "pending", "accepted", "blocked"
    public DateTime CreatedAt { get; set; }

    public User User { get; set; }
    public User Friend { get; set; }
}
