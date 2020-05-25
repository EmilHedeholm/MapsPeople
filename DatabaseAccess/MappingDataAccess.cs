using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace DatabaseAccess {
    public class MappingDataAccess {

        string constring { get; set; }

        public MappingDataAccess() {
            constring = @"data Source = .\SQLEXPRESS; database = mapping; integrated security=True";
        }

        public List<MappingEntry> GetAll() {
            using (SqlConnection connection = new SqlConnection(constring)) {
                connection.Open();
                return connection.Query<MappingEntry>("select * from entries").ToList();
            }
        }

        public MappingEntry FindById(string id) {
            using (SqlConnection connection = new SqlConnection(constring)) {
                connection.Open();
                return connection.Query<MappingEntry>("select * from entries where Id = @id", new { id }).FirstOrDefault();
            }
        }

        public void Update(MappingEntry entry) {
            using (SqlConnection connection = new SqlConnection(constring)) {
                connection.Open();
                connection.Execute("update entries set Id = @Id, ConsumerId = @ConsumerId, ExternalId = @ExternalId where Id = @Id", entry);
            }
        }

        public void Insert(MappingEntry entry) {
            using (SqlConnection connection = new SqlConnection(constring)) {
                try {
                    connection.Open();
                    connection.Execute("insert into entries (Id, ConsumerId, ExternalId) values(@Id, @ConsumerId, @ExternalId)", entry);
                }catch(SqlException se) {
                    throw new Exception();
                }
            }
        }
    }
}
