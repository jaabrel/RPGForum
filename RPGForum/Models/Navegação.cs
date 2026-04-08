namespace RPGForum.Models
{

    public ICollection<Build> Builds { get; set; } = new List<Build>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Like> Likes { get; set; } = new List<Like>();


}
