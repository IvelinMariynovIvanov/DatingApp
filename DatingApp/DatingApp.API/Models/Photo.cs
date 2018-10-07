
namespace DatingApp.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Mvc;
    
    public class Photo
    {
        public int Id {get;set;}

        public string Url {get;set;}

       // [Column("Description")]
        public string Descripton {get;set;}

        public DateTime DateAdded {get; set;}

        public bool IsMain { get; set;}

        public User User {get;set;}

        public int UserId {get;set;}
    }
}