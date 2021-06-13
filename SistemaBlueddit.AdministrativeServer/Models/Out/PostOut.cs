using System;
using System.Collections.Generic;
using SistemaBlueddit.Domain;

namespace SistemaBlueddit.AdministrativeServer.Models
{
    public class PostOut
    {
        public List<Post> Posts { get; set; }
        public string Message { get; set; }
    }
}