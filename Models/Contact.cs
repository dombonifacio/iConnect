namespace iConnect.Models
{
    public class Contact
    {
        public int ContactID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Organization { get; set; }
        public string Relationship { get; set; }
        public bool IsFavorite { get; set; }


    }
}
