public class Post
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User Author { get; set; }
    //public ICollection<Comment> Comments { get; set; }
}