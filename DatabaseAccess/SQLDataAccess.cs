using System;
using System.Collections.Generic;
using DataModels;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace DatabaseAccess {
    public class SQLDataAccess : IDataAccess {
        private string conString;
        private string clientConString;

        public SQLDataAccess() {
            conString = @"data Source = .\SQLEXPRESS; database = MapsPeople; integrated security=True";
            SqlConnection conn = new SqlConnection(conString);
            //conString = ConfigurationManager.ConnectionStrings["Con"].ConnectionString;
            //clientConString = ConfigurationManager.ConnectionStrings["ClientConnection"].ConnectionString;
        }

        public void CreateLocation(Location location) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                findSourcesByLocationID(location);
                var sql = "INSERT INTO LocationMP (id, parentId, externalId, consumerId) VALUES (@id, @parentId, @externalId, @consumerId);";
                connection.Execute(sql, location);

                CreateSource(location);
            }
        }

        private List<Source> findSourcesByLocationID(Location location) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                return connection.Query<Source>("SELECT * FROM Source WHERE locationId =@locationId").ToList();
            }
        }

        private void CreateSource(Location location) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                var sources = findSourcesByLocationID(location);
                foreach (var source in sources) {
                    var foundStates = findStatesBySource(source);
                    source.State.AddRange(foundStates);
                }

                var foundSources = "SELECT * FROM LocationMP WHERE locationId = @locationId, type = @type";
                connection.Execute(foundSources, location);
                var updateSource = "UPDATE Source SET timeStamp WHERE locationId = @locationId, type = @type";
                connection.Execute(updateSource, location);
                var sql = "INSERT INTO Source (locationId, type, timeStamp) VALUES (@locationId, @type, @timeStamp);";
                connection.Execute(sql, location);
            }
        }

        private List<State> findStatesBySource(Source source) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                return connection.Query<State>("Select * From State Where source = @locationId + @type").ToList();
            } 
        }

        private void CreateState(Source source) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                
                var sql = "SELECT* FROM Source WHERE locationId = @locationId";

                connection.Execute;
            }
        }

        public void DeleteLocationAndSubLocations(string locationId) {
            throw new NotImplementedException();
        }

        public void DeleteLocationsByConsumerId(int consumerid) {
            throw new NotImplementedException();
        }

        public List<Location> GetAllConnectedLocations(string id, List<Location> foundLocations) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                var locations = connection.Query<Location>("select * from(select Id as Id union all select parentId) as X where Id in (@Id, @ParentId)").ToList();
                foreach (var location in locations) {
                    if (location.Id.Equals(id)) {
                        foundLocations.Add(location);
                    } else {
                        foundLocations.Add(location);
                        GetAllConnectedLocations(location.Id, foundLocations);
                    }
                }
                return foundLocations;
            }
        }
        public Location GetLocationByExternalId(string externalId) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                return connection.Query<Location>("SELECT Id, parentId, externalId, consumerId FROM LocationMP WHERE externalId = @externalId", new { externalId }).SingleOrDefault();
            }
        }

        public Location GetLocationById(string id) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                return connection.Query<Location>("SELECT Id, parentId, externalId, consumerId FROM LocationMP WHERE id = @id", new { id }).SingleOrDefault();
            }
        }

        public List<Location> GetLocations() {
            using (SqlConnection connection = new SqlConnection(conString)) {
                return connection.Query<Location>("SELECT Id, parentId, externalId, consumerId FROM LocationMP").ToList();
            }
        }

        public void UpdateLocation(Location location) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                var sql = "UPDATE LocationMP SET parentId = @parentId, externalId = @externalId, consumerId = @consumerId  WHERE id = @id;";
                connection.Execute(sql, location);
            }
        }
    }
}
