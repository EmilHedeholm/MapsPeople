using System;
using System.Collections.Generic;
using DataModels;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;

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
                var sql = "INSERT INTO LocationMP (id, parentId, externalId, consumerId) VALUES (@id, @parentId, @externalId, @consumerId);";
                connection.Execute(sql, location);
            }
        }

        public void DeleteLocationAndSubLocations(string locationId) {
            throw new NotImplementedException();
        }

        public void DeleteLocationsByConsumerId(int consumerid) {
            throw new NotImplementedException();
        }

        public List<Location> GetAllConnectedLocations(string id, List<Location> foundLocations) {
            throw new NotImplementedException();
        }

        public Location GetLocationByExternalId(string externalId) {
            throw new NotImplementedException();
        }

        public Location GetLocationById(string id) {
            throw new NotImplementedException();
        }

        public List<Location> GetLocations() {
            throw new NotImplementedException();
        }

        public void UpdateLocation(Location location) {
            throw new NotImplementedException();
        }
    }
}
