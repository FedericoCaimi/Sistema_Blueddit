using SistemaBlueddit.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SistemaBlueddit.Domain
{
    public class Response: ISerializable<Response>
    {
        public string ServerResponse { get; set; }

        public Response DeserializeObject(string objectToDeserialize)
        {
            var pattern = new Regex("{|}");
            var objectProperty = pattern.Replace(objectToDeserialize, "").Split(":");
            ServerResponse = objectProperty[1];
            return this;
        }

        public string SerializeObejct()
        {
            return $"{{response:{ServerResponse}}}";
        }
    }
}
