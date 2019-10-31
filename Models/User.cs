using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [MinLength(2, ErrorMessage="Your name is not long enough. 2(two) characters minimum.")]
        public string FirstName {get;set;}

        [Required]
        [MinLength(2, ErrorMessage="Your name is not long enough. 2(two) characters minimum.")]
        public string LastName {get;set;}

        [EmailAddress]
        [Required]
        public string Email {get;set;}

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("Password", ErrorMessage="Yo! That didn't match!")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
    }

    public class LogUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password { get; set; }
    }

    public class UindexView
    {
        public User UserNew {get; set;}
        public LogUser UserExist {get; set;}
    }
}
