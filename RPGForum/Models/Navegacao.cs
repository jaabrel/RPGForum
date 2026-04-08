namespace RPGForum.Models
{

    public ICollection<BuildPost> Builds { get; set; } = new List<BuildPost>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Like> Likes { get; set; } = new List<Like>();


}
