namespace DatingApp.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Mvc;
    public class Like
    {
        public int LikerId {get;set;}

        public int LikeeId {get;set;}

        public User Liker {get;set;}

        public User Likee {get;set;}
        
    }
}