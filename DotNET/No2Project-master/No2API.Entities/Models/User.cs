using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Entities.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        //public string Username { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Lütfen geçerli bir email adresi girin.")]
        [Required(ErrorMessage = "Email girmek zorunludur.")]
        public string Email { get; set; }
        [DataType(DataType.Password, ErrorMessage = "Lütfen geçerli bir şifre adresi girin.")]
        [Required(ErrorMessage = "Şifre girmek zorunludur.")]
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public string Name { get; set; }
        public string Surname { get; set; }      
        public string Gender { get; set; }
        public string Phone { get; set; }
        //public string Location { get; set; }
        public bool HasPaid { get; set; } = false;
        public bool IsPremium { get; set; } = false;
        public string Subscription { get; set; }
        public string Type { get; set; } //human or pet
        public string City { get; set; }
        public int Views { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime ActivationDate { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime ExpirationDate { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime DateOfBirth { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedAt { get; set; }
        //[NotMapped]
        public string Token { get; set; }
        public bool ConfirmedEmail { get; set; } = false;
        public string MailConfirmationToken { get; set; }
        public string ResetPasswordToken { get; set; }
        public virtual User Referrer { get; set; }
        public virtual List<CouponCode> CouponCodes { get; set; }
        public virtual List<Picture> Pictures { get; set; }
        public virtual List<Video> Videos { get; set; }
        public virtual List<Order> Orders { get; set; }
    }
}
