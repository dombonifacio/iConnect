using iConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace iConnect.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = "SELECT * FROM Contact";
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                do
                {

                    while (dataReader.Read())
                    {
                        // HARD CODED WAY OF READING DATA
                        //int contactID = Convert.ToInt32(dataReader["ContactID"]);
                        //string firstName = Convert.ToString(dataReader["FirstName"]);
                        //string lastName = Convert.ToString(dataReader["LastName"]);
                        //string email = Convert.ToString(dataReader["Email"]);
                        //string phone = Convert.ToString(dataReader["Phone"]);
                        //string organization = Convert.ToString(dataReader["Organization"]);
                        //string relationship = Convert.ToString(dataReader["Relationship"]);
                        //string group = Convert.ToString(dataReader["Group"]);

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            string currentColName = dataReader.GetName(i);
                            string currentColVal = Convert.ToString(dataReader.GetValue(i));
                        }


                    }
                } while (dataReader.NextResult());
            }
            connection.Close();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
