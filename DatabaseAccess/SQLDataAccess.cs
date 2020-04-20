using System;
using System.Collections.Generic;
using DataModels;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Data;

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

                CreateSources(location);
            }
        }

        private List<Source> findSourcesByLocationID(Location location) {
            List<Source> sources = new List<Source>();
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                using (SqlCommand cmdFoundSource = connection.CreateCommand()) {
                    cmdFoundSource.CommandText = "SELECT * FROM Source WHERE locationId = @locationId";
                    cmdFoundSource.Parameters.AddWithValue("@locationId", location.Id);
                    SqlDataReader sourceReader = cmdFoundSource.ExecuteReader();
                    while (sourceReader.Read()) {
                        object[] values = new object[5];
                        var columns = sourceReader.GetValues(values);

                        sources.Add(MapSource(sourceReader));
                    }

                }
            }
            foreach (var source in sources) {
                var foundStates = findStatesBySource(source, location);
                source.State.AddRange(foundStates);
            }

            return sources;
        }

        private Source MapSource(IDataReader sourceReader) {
            return new Source {
                Type = sourceReader.GetString(1),
                TimeStamp = sourceReader.GetDateTime(2)
            };
        }

        private void CreateSources(Location location) {
           
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                foreach (Source source in location.Sources) {
                    var sources = findSourcesByLocationID(location);
                    if (sources.Count() == 0) {
                        Source source2 = new Source {
                            Type = source.Type,
                            TimeStamp = source.TimeStamp
                        };
                        
                        using (SqlCommand cmdInsertSource = connection.CreateCommand()) {
                            cmdInsertSource.CommandText = "INSERT INTO Source(locationId, type, timeStamp) VALUES(@locationId, @type,@timeStamp)";
                            cmdInsertSource.Parameters.AddWithValue("locationId", location.Id);
                            cmdInsertSource.Parameters.AddWithValue("type", source.Type);
                            cmdInsertSource.Parameters.AddWithValue("timeStamp", source.TimeStamp);
                            cmdInsertSource.ExecuteNonQuery();

                        }
                    }
                    CreateStates(source, location);
                }

                //var foundSources = "SELECT * FROM LocationMP WHERE locationId = @locationId, type = @type";
                //connection.Execute(foundSources, location);
                //var updateSource = "UPDATE Source SET timeStamp WHERE locationId = @locationId, type = @type";
                //connection.Execute(updateSource, location);
                //var sql = "INSERT INTO Source (locationId, type, timeStamp) VALUES (@locationId, @type, @timeStamp);";
                //connection.Execute(sql, location);
            }
        }

        private List<State> findStatesBySource(Source source, Location location) {
            List<State> states = new List<State>();
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                using (SqlCommand cmdFoundStates = connection.CreateCommand()) {
                    cmdFoundStates.CommandText = "SELECT * FROM State WHERE source = @source";
                    cmdFoundStates.Parameters.AddWithValue("@source", location.Id + source.Type);
                    SqlDataReader stateReader = cmdFoundStates.ExecuteReader();
                    while (stateReader.Read()) {
                        object[] values = new object[5];
                        var columns = stateReader.GetValues(values);

                        states.Add(MapState(stateReader));
                    }

                }
            }
            return states;
        }

        private State MapState(IDataReader stateReader) {
            return new State {
                Property = stateReader.GetString(1),
                Value = stateReader.GetString(2)
            };
        }

        private void CreateStates(Source source, Location location) {
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
