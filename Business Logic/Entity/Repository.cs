using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BucketProject.BLL.Business_Logic.Entity
{
    public class Repository
    {
        private readonly string _connectionString;

        protected Repository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
