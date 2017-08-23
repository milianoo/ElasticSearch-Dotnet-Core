using System.Collections.Generic;
using Nest;

namespace ElasticQuery
{
    public class Person
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        
        public List<string> Skills { get; set; }
        
        public Dictionary<string, string> Others { get; set; }
        
        public Dictionary<string, dynamic> OthersDynamic { get; set; }
    }
}