namespace HelperLibrary.ForumSystem
{
    public class PostVoteType
    {
        public string Value { get; set; }

        public readonly PostVoteType Like = new PostVoteType
        {
            Value = "like"
        };

        public readonly PostVoteType Dislike = new PostVoteType
        {
            Value = "dislike"
        };
    }
}