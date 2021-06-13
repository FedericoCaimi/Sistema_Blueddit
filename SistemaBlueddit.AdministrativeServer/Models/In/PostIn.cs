using System;
using System.Collections.Generic;
using SistemaBlueddit.Domain;

namespace SistemaBlueddit.AdministrativeServer.Models
{
    public class PostIn
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public List<Topic> Topics { get; set; }

    }
}