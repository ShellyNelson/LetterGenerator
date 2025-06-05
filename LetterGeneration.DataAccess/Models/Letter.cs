namespace LetterGeneration.DataAccess.Models
{
    public class Letter
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? SentDate { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Sent, Archived
    }
} 