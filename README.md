## Elastic Search using NEST library in dotnet core 1.1


This is a sample project to test some approaches for Querying and Searching on Elastic search using NEST client. 

### Interesting Points

- Search over List<string>
- Search over Dictionary<string, string>
- Search over Dictionary<string, dynamic>

### Need to setup Elastic Search environment ?

I used the ELK docker setup by @antoineco available at [here](https://github.com/deviantony/docker-elk) 

after lunch please visit http://127.0.0.1:9200 

However I made some small adjustments on my environment, to make it simpler, if you are interested use the files available in the ELK directory. and Run `docker-compose up`
