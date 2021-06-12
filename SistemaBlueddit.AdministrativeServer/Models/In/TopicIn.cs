using System;
using System.Collections.Generic;
using SistemaBlueddit.Domain;

namespace SistemaBlueddit.AdministrativeServer.Models
{
    public class TopicIn
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public TopicIn()
        {
        }

        public Topic ToEntity() => new Topic()
        {
            Name = this.Name,
            Description = this.Description
        };

    }
}