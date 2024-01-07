namespace Domain
{
  public class UserFollowing
  {
    // Instead of using follower and following user
    // we use Observer (follower) and Target (following user)
    public string ObserverId { get; set; }
    public AppUser Observer { get; set; }
    public string TargetId { get; set; }
    public AppUser Target { get; set; }
  }
}