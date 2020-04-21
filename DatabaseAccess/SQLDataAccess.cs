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

        public SQLDataAccess() {
            conString = @"data Source = .\SQLEXPRESS; database = MapsPeopleDB; integrated security=True";
            SqlConnection conn = new SqlConnection(conString);
        }

        public void CreateLocation(Location location) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                var sql = "INSERT INTO LocationMP (id, parentId, externalId, consumerId) VALUES (@id, @parentId, @externalId, @consumerId);";
                connection.Execute(sql, location);

                CreateSources(location);
            }
        }

        private List<Source> FindSourcesByLocationID(Location location) {
            List<Source> sources = new List<Source>();
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                using (SqlCommand cmdFoundSource = connection.CreateCommand()) {
                    cmdFoundSource.CommandText = "SELECT * FROM Source WHERE locationId = @locationId";
                    cmdFoundSource.Parameters.AddWithValue("@locationId", location.Id);
                    SqlDataReader sourceReader = cmdFoundSource.ExecuteReader();
                    while (sourceReader.Read()) {
                        sources.Add(MapSource(sourceReader));
                    }
                }
            }
            foreach (var source in sources) {
                var foundStates = FindStatesBySource(source, location);
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
                    using (SqlCommand cmdInsertSource = connection.CreateCommand()) {
                        cmdInsertSource.CommandText = "INSERT INTO Source(locationId, sourceType, sourceTimeStamp) VALUES(@locationId, @sourceType, @sourceTimeStamp)";
                        cmdInsertSource.Parameters.AddWithValue("@locationId", location.Id);
                        cmdInsertSource.Parameters.AddWithValue("@sourceType", source.Type);
                        cmdInsertSource.Parameters.AddWithValue("@sourceTimeStamp", source.TimeStamp);
                        cmdInsertSource.ExecuteNonQuery();
                    }
                    CreateStates(source, location);
                }
            }
        }

        private List<State> FindStatesBySource(Source source, Location location) {
            List<State> states = new List<State>();
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                using (SqlCommand cmdFoundStates = connection.CreateCommand()) {
                    cmdFoundStates.CommandText = "SELECT * FROM StateMP WHERE locationId = @LocationId AND sourceType = @sourceType ";
                    cmdFoundStates.Parameters.AddWithValue("@locationId", location.Id);
                    cmdFoundStates.Parameters.AddWithValue("@sourceType", source.Type);
                    SqlDataReader stateReader = cmdFoundStates.ExecuteReader();
                    while (stateReader.Read()) {
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
                connection.Open();
                foreach (State state in source.State) {
                    using (SqlCommand cmdInsertState = connection.CreateCommand()) {
                        cmdInsertState.CommandText = "INSERT INTO StateMP(locationId, sourceType, property, stateValue) VALUES(@locationId, @sourceType, @property, @stateValue)";
                        cmdInsertState.Parameters.AddWithValue("@locationId", location.Id);
                        cmdInsertState.Parameters.AddWithValue("@sourceType", source.Type);
                        cmdInsertState.Parameters.AddWithValue("@property", state.Property);
                        cmdInsertState.Parameters.AddWithValue("@stateValue", state.Value);
                        cmdInsertState.ExecuteNonQuery();
                    }
                }
            }
        }
        public void DeleteLocationAndSubLocations(string locationId) {
            throw new NotImplementedException();
        }

        public void DeleteLocationsByConsumerId(int consumerid) {
            throw new NotImplementedException();
        }

        public List<Location> GetAllConnectedLocations(string id, List<Location> foundLocations) {
            List<Location> locations = new List<Location>();
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                using (SqlCommand cmdGetAllConnectedLocations = connection.CreateCommand()) {
                    cmdGetAllConnectedLocations.CommandText = "SELECT * FROM LocationMP WHERE id=@id OR parentId=@id2";
                    cmdGetAllConnectedLocations.Parameters.AddWithValue("@id", id);
                    cmdGetAllConnectedLocations.Parameters.AddWithValue("@id2", id);
                    SqlDataReader locationReader = cmdGetAllConnectedLocations.ExecuteReader();
                    while (locationReader.Read()) {
                        locations.Add(MapLocation(locationReader));
                    }
                }
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

        private Location MapLocation(IDataReader locationReader) {
            Location location = new Location {
                Id = locationReader.GetString(0),
                ParentId = locationReader.GetString(1),
                ExternalId = locationReader.GetString(2),
                ConsumerId = locationReader.GetInt32(3)
            };
            location.Sources = FindSourcesByLocationID(location);
            return location;
        }

        public Location GetLocationByExternalId(string externalId) {
            Location location = null;
            using (SqlConnection connection = new SqlConnection(conString)) {
                location = connection.Query<Location>("SELECT Id, parentId, externalId, consumerId FROM LocationMP WHERE externalId = @externalId", new { externalId }).SingleOrDefault();
                location.Sources = FindSourcesByLocationID(location);
                return location;
            }
        }
        public Location GetLocationById(string id) {
            Location location = null;
            using (SqlConnection connection = new SqlConnection(conString)) {
                location = connection.Query<Location>("SELECT id, parentId, externalId, consumerId from locationMP WHERE id = @id", new { id }).SingleOrDefault();
                location.Sources = FindSourcesByLocationID(location);
                return location;
            }
        }

        public List<Location> GetLocations() {
            List<Location> locations = new List<Location>();
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                using (SqlCommand cmdFoundAllLocation = connection.CreateCommand()) {
                    cmdFoundAllLocation.CommandText = "SELECT * FROM LocationMP";
                    SqlDataReader foundAllReader = cmdFoundAllLocation.ExecuteReader();
                    while (foundAllReader.Read()) {
                        locations.Add(MapLocation(foundAllReader));
                    }
                }
            }
            foreach (var location in locations) {
                location.Sources = FindSourcesByLocationID(location);
            }
            return locations;
        }

        public void UpdateLocation(Location location) {
            using (SqlConnection connection = new SqlConnection(conString)) {
                connection.Open();
                var sql = "UPDATE LocationMP SET parentId = @parentId, externalId = @externalId, consumerId = @consumerId  WHERE id = @id;";
                connection.Execute(sql, location);

                List<Source> sources = FindSourcesByLocationID(location);
                if (sources.Count > 0) {
                    foreach (var source in sources) {
                        using (SqlCommand updateSources = connection.CreateCommand()) {
                            var sourceSQL = "UPDATE source SET sourceTimestamp = @Timestamp WHERE locationId = @locationId AND sourceType = @sourceType";
                            updateSources.CommandText = sourceSQL;
                            updateSources.Parameters.AddWithValue("@Timestamp", source.TimeStamp);
                            updateSources.Parameters.AddWithValue("@locationId", location.Id);
                            updateSources.Parameters.AddWithValue("@sourceType", source.Type);
                            updateSources.ExecuteNonQuery();
                            List<State> states = FindStatesBySource(source, location);
                            if (states.Count > 0) {
                                foreach (var state in states) {
                                    using (SqlCommand updateStates = connection.CreateCommand()) {
                                        var stateSQL = "UPDATE stateMP SET stateValue = @stateValue WHERE locationId = @locationId AND sourceType = @sourceType AND property = @property";
                                        updateStates.CommandText = stateSQL;
                                        updateStates.Parameters.AddWithValue("@property", state.Property);
                                        updateStates.Parameters.AddWithValue("@stateValue", state.Value);
                                        updateStates.Parameters.AddWithValue("@locationId", location.Id);
                                        updateStates.Parameters.AddWithValue("@sourceType", source.Type);
                                        updateStates.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                } else CreateSources(location);
            }
        }
    }
}
