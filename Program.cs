using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Nest;

namespace ElasticQuery
{
    class Program
    {
        private static void Log(string text)
        {
            Console.WriteLine(text);
        }

        private static ElasticClient _client;

        private static void Connect()
        {
            var settings = new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .DefaultIndex("people");

            var client = new ElasticClient(settings);
            
            _client = client;
        }

        static void DeleteIndexes()
        {
            _client.DeleteIndex(Indices.All);
        }

        static void IndexPersons()
        {
            var person1 = new Person
            {
                Id = 1,
                FirstName = "Milad",
                LastName = "Rezazadeh", 
                Skills = new List<string>() { "Communication", "Talking", "Fun" },
                Others = new Dictionary<string, string>(),
                OthersDynamic = new Dictionary<string, dynamic>()
            };
            person1.Others.Add("Phone", "94392302");
            person1.Others.Add("Address", "South town 202");
            person1.Others.Add("Email", "milad@rezazadeh.com");
            person1.OthersDynamic.Add("BBBB", "888888");
            
            var person2 = new Person
            {
                Id = 2,
                FirstName = "Milad",
                LastName = "Gjuroski", 
                Skills = new List<string>() { "Communication", "Speaking", "Humor" },
                Others = new Dictionary<string, string>()
            };
            person2.Others.Add("Phone", "233223");
            person2.Others.Add("Address", "shomal town 101");
            person2.Others.Add("Email", "marjan@livedooh.com");

            _client.Index(person1, indexDescriptor =>
                indexDescriptor
                    .Index("people")
                    .Id(person1.Id)
                .Refresh(Refresh.True));
            _client.Index(person2, indexDescriptor =>
                indexDescriptor
                    .Index("people")
                    .Id(person2.Id)
                    .Refresh(Refresh.True));
        }

        static void QueyrDistinct()
        {
            var result = _client.Search<Person>(s => s
                .Size(0)
                .Aggregations(a => a
                    .Terms("firstname", t => t.Field(f => f.FirstName))
                    .Terms("lastname", t => t.Field(f => f.LastName))
                )
                .Query(q => q
                    .MatchAll()
                )
            );
            
            var uniqueOwnerUserIds = result.Aggs.Terms("FirstNames").Buckets.Select(b => b.KeyAsString).ToList();

            if (result.IsValid)
            {
                var outputs = result.Documents;
                Log($"Queyr Distinct Found {outputs.Count} results");
                foreach (var output in outputs)
                {
                    Log($"\t - {output.FirstName}");
                } 
            }
        }
        
        static void SearchDictionaryKeyExist()
        {
            var searchResponse = _client.Search<Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Exists(m => m.Field(f => f.Others["Address"]))
                )
            );

            var people = searchResponse.Documents;
            
            Log($"Search Dictionary Key Using Match Query Found {people.Count} results");
        }
        
        static void SearchDictionaryUsingMatch()
        {
            var searchResponse = _client.Search<Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Others["Address"])
                        .Query("shomal")
                    )
                )
            );

            var people = searchResponse.Documents;
            
            Log($"Search Dictionary Using Match Query Found {people.Count} results");
        }
        
        static void QueryFirstNameOnlyAndSorting()
        {
            var searchResponse = _client.Search<Person>(s => s
                .Sort(st=> st.Ascending(a => a.Id))
                .Source(sf =>sf 
                    .Includes(i => i 
                        .Fields(
                            f => f.Id,
                            f => f.FirstName
                        )
                    )
                )
                .Query(q => q
                    .MatchAll()
                )
            );

            var people = searchResponse.Documents;
            
            Log($"Query First Name Found {people.Count} results");

            foreach (var person in people)
            {
                Log($"\t - {person.Id} {person.FirstName} {person.LastName}");
            }
            
        }

        static void SearchDictionaryUsingMatchPhrasePrefix()
        {
            var searchResponse = _client.Search<Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .MatchPhrasePrefix(c => 
                        c.Field(f => f.Others["Address"])
                            .Query("2"))
                )
            );

            var people = searchResponse.Documents;
            
            Log($"Search Dictionary<string,string> Using MatchPhrasePrefix : Found {people.Count} results");
        }
        
        static void SearchDynamicUsingMatchPhrasePrefix()
        {
            var searchResponse = _client.Search<Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .MatchPhrasePrefix(c => 
                        c.Field(f => f.OthersDynamic["BBBB"])
                            .Query("8"))
                )
            );

            var people = searchResponse.Documents;
            
            Log($"Search Dictionary<string,dynamic> Using MatchPhrasePrefix : Found {people.Count} results");
        }
        
        static void Main(string[] args)
        {
            Log("MatchPhrasePrefix Docs: ");
            Log("https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/match-phrase-prefix-usage.html\n");
            
            Log("start Elastic Query ...");

            Connect();
            
            Log("Elastic Query connected.");

            IndexPersons();
//            DeleteIndexes();

//            QueyrDistinct(); 
            
            QueryFirstNameOnlyAndSorting();

            SearchDictionaryKeyExist();
            SearchDictionaryUsingMatch();
            SearchDictionaryUsingMatchPhrasePrefix();
            SearchDynamicUsingMatchPhrasePrefix();
        }
    }
}
