using System;
using System.Collections.Generic;

namespace AUEUMS.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string UserManager { get; set; }
        public string PhoneNo { get; set; }
        public string Designation { get; set; }
        public string Extension { get; set; }
        public Department Department { get; set; }
        public bool? DeActivated { get; set; }
        public string AuthToken { get; set; }        
        public string ImgUrl { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RefreshToken { get; set; }
        public string FullName
        {
            get
            {

                return FirstName + " " + LastName;
            }
        }

        public string SecretKey { get; set; }    
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? UserApproved { get; set; }
        public int UserApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public College College { get; set; }
        
    }

    public class UserUpdateModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? LastLoggedIn { get; set; }
    }
    public class UserView
    {
        public string tokenId { get; set; }
    }
    public class UserReg
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class UserResource
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public string message { get; set; }
        public bool success { get; set; }
        public string FirstName { get; set; }
        public string designation { get; set; }
    


    }
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }       
        public string FullName
        {
            get
            {

                return FirstName + " " + LastName;
            }
        }
    }
}
