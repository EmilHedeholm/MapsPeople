using System;
using System.Collections.Generic;
using DataModels;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Data;

namespace DatabaseAccess {
    //This class uses a MSSQL database.
    public class SQLDataAccess : IDataAccess {
        private string conString;

        //this constructor initialises the connectionstring
        public SQLDataAccess() {
            conString = @"data Source = .\SQLEXPRESS; database = MapsPeopleDB; integrated security=True";
            //SqlConnection conn = new SqlConnection(conString);
        }

        //this method inserts a location with all its sources into the database. It uses Dapper.
        public void CreateLocation(Location location) {
            SqlConnection connection = null;
            try {
                using (connection = new SqlConnection(conString)) {
                    connection.Open();
                    var sql = "INSERT INTO LocationMP (id, parentId, externalId, consumerId) VALUES (@id, @parentId, @externalId, @consumerId);";
                    connection.Execute(sql, location);
                    CreateSources(location);
                }
            } catch (SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to insert a location into the database", se);
            }  
        }

        //this method finds sources in the database based on the location they belong to
        private List<Source> FindSourcesByLocationID(Location location) {
            List<Source> sources = new List<Source>();
            SqlConnection connection = null;
            try {
                using (connection = new SqlConnection(conString)) {
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
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to find a source", se);
            }
            return sources;
        }

        //this method maps data from a datareader to source objects
        private Source MapSource(IDataReader sourceReader) {
            Source source = new Source();
            if (!sourceReader.IsDBNull(1)) {
                source.Type = sourceReader.GetString(1);
            } else {
                source.Type = null;
            }
            if (!sourceReader.IsDBNull(2)) {
                source.TimeStamp = sourceReader.GetDateTime(2);
            } else {
                source.TimeStamp = DateTime.Now;
            }
            return source;
        }

        //this method inserts sources with all its states into the database 
        private void CreateSources(Location location) {
            SqlConnection connection = null;
            try { 
            using (connection = new SqlConnection(conString)) {
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
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to insert a source into the database", se);
            }
        }

        //this method find states in the database based on the source they belong to
        private List<State> FindStatesBySource(Source source, Location location) {
            List<State> states = new List<State>();
            SqlConnection connection = null;
            try {
                using (connection = new SqlConnection(conString)) {
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
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to find a state", se);
            }
            return states;
        }

        //this method maps data from a datareader to state objects
        private State MapState(IDataReader stateReader) {
            State state = new State();
            if (!stateReader.IsDBNull(1)) {
                state.Property = stateReader.GetString(1);
            } else {
                state.Property = null;
            }
            if (!stateReader.IsDBNull(2)) {
                state.Value = stateReader.GetString(2);
            } else {
                state.Value = null;
            }
            return state;
        }
         //this method inserts states into the database
        private void CreateStates(Source source, Location location) {
            SqlConnection connection = null;
            try {
                using (connection = new SqlConnection(conString)) {
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
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to insert a state into the database", se);
            }
        }

        //this method find a location and all the locations below it in the hierarki of locations
        public HashSet<Location> GetAllConnectedLocations(string id, HashSet<Location> foundLocations) {
            SqlConnection connection = null;
            HashSet<Location> locations = new HashSet<Location>();
            try {
                using (connection = new SqlConnection(conString)) {
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
                }
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to find a location", se);
            }
            return foundLocations;
        }

        //this method maps data from a datareader to location objects
        private Location MapLocation(IDataReader locationReader) {
            Location location = new Location();
            if (!locationReader.IsDBNull(0)) {
                location.Id = locationReader.GetString(0);
            } else {
                location.Id = null;
            }
            if (!locationReader.IsDBNull(1)) {
                location.ConsumerId = locationReader.GetInt32(1);
            } else {
                location.ConsumerId = 0;
            }
            if (!locationReader.IsDBNull(2)) {
                location.ParentId = locationReader.GetString(2);
            } else {
                location.ParentId = null;
            }
            if (!locationReader.IsDBNull(3)) {
                location.ExternalId = locationReader.GetString(3);
            } else {
                location.ExternalId = null;
            }
            location.Sources = FindSourcesByLocationID(location);
            return location;
        }

        //this method finds a location based on its externalId
        public Location GetLocationByExternalId(string externalId) {
            Location location = null;
            SqlConnection connection = null;
            try {
                using (connection = new SqlConnection(conString)) {
                    connection.Open();
                    location = connection.Query<Location>("SELECT Id, parentId, externalId, consumerId FROM LocationMP WHERE externalId = @externalId", new { externalId }).SingleOrDefault();
                    if (location != null) {
                        location.Sources = FindSourcesByLocationID(location);
                    }
                }
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to find a location", se);
            }
            return location;
        }

        //this method finds a location based on its id
        public Location GetLocationById(string id) {
            Location location = null;
            SqlConnection connection = null;
            try {
                using (connection = new SqlConnection(conString)) {
                    connection.Open();
                    location = connection.Query<Location>("SELECT id, parentId, externalId, consumerId from locationMP WHERE id = @id", new { id }).SingleOrDefault();
                    if (location != null) { 
                    location.Sources = FindSourcesByLocationID(location);
                    }
                }
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to find a location",se);
            }
            return location;
        }

        //This method updates a locations source, and if the source does not exists then it creates the source.
        public void UpdateLocation(Location location) {
            SqlConnection connection = null;
            try {
                using (connection = new SqlConnection(conString)) {
                    connection.Open();
                    var sql = "UPDATE LocationMP SET parentId = @parentId, externalId = @externalId, consumerId = @consumerId  WHERE id = @id;";
                    connection.Execute(sql, location);

                    List<Source> sources = FindSourcesByLocationID(location);
                    if (sources.Count > 0) {
                        foreach (var source in sources) {
                            //Updates source
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
                                        // Updates state
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
                        //If there are no sources on the location then create the source. 
                    } else CreateSources(location);
                }
            } catch(SqlException se) {
                connection.Dispose();
                throw new Exception("something went wrong when trying to update a location", se);
            }
        }
    }
}
