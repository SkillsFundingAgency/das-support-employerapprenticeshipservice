
namespace SFA.DAS.Support.Shared
{

    public class UserSearchModel
    {
        public string Id { get; set; }
        public string Name
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public string Email { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public SearchCategory SearchType { get; set; }
    }
}