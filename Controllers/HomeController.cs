using iConnect.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

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
            return View();
        }

        public IActionResult Contacts()
        {
            List<Contact> contactList = new List<Contact>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Fetch the list of contacts
                string contactSql = "SELECT * FROM Contact";
                using (SqlCommand command = new SqlCommand(contactSql, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        Contact contact = new Contact
                        {
                            ContactID = Convert.ToInt32(dataReader["ContactID"]),
                            FirstName = Convert.ToString(dataReader["FirstName"]),
                            LastName = Convert.ToString(dataReader["LastName"]),
                            Email = Convert.ToString(dataReader["Email"]),
                            Phone = Convert.ToString(dataReader["Phone"]),
                            Organization = Convert.ToString(dataReader["Organization"]),
                            Relationship = Convert.ToString(dataReader["Relationship"])
                        };

                        // Check if the contact is in the favorites list
                        string favoritesSql = $"SELECT COUNT(*) FROM Favourites WHERE ContactID='{contact.ContactID}'";
                        using (SqlCommand favoritesCommand = new SqlCommand(favoritesSql, connection))
                        {
                            int count = (int)favoritesCommand.ExecuteScalar();
                            contact.IsFavorite = count > 0;
                        }

                        contactList.Add(contact);
                    }

                    connection.Close();
                }
            }

            return View(contactList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Contact contact)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "INSERT INTO Contact (FirstName, LastName, Email, Phone, Organization, Relationship) " +
							 "VALUES (@FirstName, @LastName, @Email, @Phone, @Organization, @Relationship);" +
							 "SELECT @Result = 'New contact has been added.';";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@FirstName", contact.FirstName);
					command.Parameters.AddWithValue("@LastName", contact.LastName);
					command.Parameters.AddWithValue("@Email", contact.Email);
					command.Parameters.AddWithValue("@Phone", contact.Phone);
					command.Parameters.AddWithValue("@Organization", contact.Organization);
					command.Parameters.AddWithValue("@Relationship", contact.Relationship);
					command.Parameters.Add("@Result", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

					connection.Open();
					command.ExecuteNonQuery();

					string result = Convert.ToString(command.Parameters["@Result"].Value);
					ViewBag.Result = result;
					connection.Close();
				}
			}

	

			return View();

        }

        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            Contact contact = new Contact();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * FROM Contact Where ContactID='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
						contact.ContactID = Convert.ToInt32(dataReader["ContactID"]);
						contact.FirstName = Convert.ToString(dataReader["FirstName"]);
						contact.LastName = Convert.ToString(dataReader["LastName"]);
						contact.Email = Convert.ToString(dataReader["Email"]);
						contact.Phone = Convert.ToString(dataReader["Phone"]);
						contact.Organization = Convert.ToString(dataReader["Organization"]);
						contact.Relationship = Convert.ToString(dataReader["Relationship"]);
					}
                }

                connection.Close();
            }

            return View(contact);
        }

        [HttpPost]
        public IActionResult Update(Contact contact, int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Update Contact SET FirstName='{contact.FirstName}', LastName='{contact.LastName}', Email='{contact.Email}', Phone='{contact.Phone}', Organization='{contact.Organization}', Relationship='{contact.Relationship}' Where ContactID='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Contacts");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From Contact Where ContactID='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return RedirectToAction("Contacts");


            }
        }

        [HttpPost]
        public IActionResult DeleteFavourites(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"Delete From Favourites Where ContactID='{id}'";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
				return RedirectToAction("Favourites");


			}
		}

        [HttpPost]
        public IActionResult DeleteFavouritesContactsPage(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From Favourites Where ContactID='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return RedirectToAction("Contacts");


            }
        }


        public IActionResult Favourites()
        {
            List<Contact> favouriteList = new List<Contact>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT C.FirstName, C.LastName, C.Email, C.Phone, C.Organization, C.Relationship
            FROM Favourites F
            INNER JOIN Contact C ON F.ContactID = C.ContactID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        // Retrieve the contact details from the query result
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();
                        string email = reader["Email"].ToString();
                        string phone = reader["Phone"].ToString();
                        string organization = reader["Organization"].ToString();
                        string relationship = reader["Relationship"].ToString();

                        // Create a new Contact object and add it to the list
                        Contact contact = new Contact
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            Phone = phone,
                            Organization = organization,
                            Relationship = relationship
                        };

                        favouriteList.Add(contact);
                    }

                    // Close the reader and connection
                    reader.Close();
                    connection.Close();
                }

                // Now you have the favouriteList containing the contact details from the favorites.
                // You can use this list to display the contacts or process them further as needed.

                // For example, you can pass the favouriteList to a view to display the favorite contacts.
                return View(favouriteList);
            }
        }
        [HttpPost]
        public IActionResult AddFavourites(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Instead of deleting the contact, add it to the Favorites table
                string insertSql = $"INSERT INTO Favourites (ContactID) VALUES ('{id}')";
                using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                {
                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                    connection.Close();
                }
                
                return RedirectToAction("Contacts");
            }
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