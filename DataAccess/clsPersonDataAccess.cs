using System;
using System.Data;
using System.Data.SqlClient;


namespace DataAccess
{
    public class clsPersonDataAccess
    {
        private static string Connection = clsDataAccessSettings.ConnectionString;

        private const string SelectQuery = @"SELECT PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName, 
                                                     DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath  
                                            FROM    People WHERE PersonID = @PersonID";

        private const string InsertQuery = @"INSERT INTO People (NationalNo, FirstName, SecondName, ThirdName, LastName, Gender, Address, DateOfBirth, 
                                                        Phone, Email, NationalityCountryID, ImagePath)
                                            VALUES (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @Gender, @Address, @DateOfBirth, 
                                                    @Phone, @Email, @NationalityCountryID, @ImagePath);
                                            SELECT SCOPE_IDENTITY();";

        private const string UpdateQuery = @"Update  People  
                        SET NationalNo = @NationalNo, 
                            FirstName = @FirstName, 
                            SecondName = @SecondName, 
                            ThirdName = @ThirdName, 
                            LastName = @LastName, 
                            Gender = @Gender, 
                            Address = @Address, 
                            DateOfBirth = @DateOfBirth, 
                            Phone = @Phone, 
                            Email = @Email, 
                            NationalityCountryID = @NationalityCountryID, 
                            ImagePath = @ImagePath
                            where PersonID = @PersonID";

        private const string AllPeopleQuery = "SELECT * FROM People";

        private const string DeletePersonQuery = @"DELETE People WHERE PersonID = @PersonID";

        private const string NationalNoExistsQuery = "SELECT Found=1 FROM People WHERE NationalNo = @NationalNo";

        private const string SelectByNationalNoQuery = @"SELECT * FROM People WHERE NationalNo = @NationalNo";

        private static string PeopleWithStringNamesQuery = @"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, 
                                                                       People.ThirdName, People.LastName, 
                                                          CASE WHEN People.Gender = 0 THEN 'Male' ELSE 'Female' END AS Gender,
                                                          People.DateOfBirth, Countries.CountryName AS Nationality, 
                                                          People.Phone, People.Email  
                                                          FROM People 
                                                          INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID";

        private static T GetValueOrDefault<T>(SqlDataReader reader, string ColmName, T DefaultValue)
        {
            var ReaderValue = reader[ColmName];
            return ReaderValue != DBNull.Value ? (T)ReaderValue : DefaultValue;
        }

        public static bool GetPersonInfoByID(int ID, ref string NationalNo, ref string FirstName, ref string SecondName, ref string ThirdName,
            ref string LastName, ref DateTime DateOfBirth, ref short Gender, ref string Address, ref string Phone,
            ref string Email, ref int NationalityCountryID, ref string ImagePath)
        {

            SqlConnection SqlConnection = new SqlConnection(Connection);

            SqlCommand command = new SqlCommand(SelectQuery, SqlConnection);

            command.Parameters.AddWithValue("@PersonID", ID);

            bool isFound = false;

            try
            {
                SqlConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;

                    // NationalNo =  (string)reader["NationalNo"]; // old style reader 

                    NationalNo = GetValueOrDefault(reader, "NationalNo", string.Empty);
                    FirstName = GetValueOrDefault(reader, "FirstName", string.Empty);
                    SecondName = GetValueOrDefault(reader, "SecondName", string.Empty);
                    ThirdName = GetValueOrDefault(reader, "ThirdName", string.Empty);
                    LastName = GetValueOrDefault(reader, "LastName", string.Empty);
                    DateOfBirth = GetValueOrDefault(reader, "DateOfBirth", DateTime.MinValue);
                    Gender = GetValueOrDefault(reader, "Gender", (short)-1);
                    Address = GetValueOrDefault(reader, "Address", string.Empty);
                    Phone = GetValueOrDefault(reader, "Phone", string.Empty);
                    Email = GetValueOrDefault(reader, "Email", string.Empty);
                    NationalityCountryID = GetValueOrDefault(reader, "NationalityCountryID", -1);
                    ImagePath = GetValueOrDefault(reader, "ImagePath", string.Empty);
                }
                else
                {
                    // The record was not found
                    isFound = false;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                SqlConnection.Close();
            }

            return isFound;
        }

        public static int AddNewPerson(string NationalNo, string FirstName, string SecondName, string ThirdName,
             string LastName, short Gender, string Address, DateTime DateOfBirth, string Phone,
             string Email, int NationalityCountryID, string ImagePath)
        {
            //this function will return the new Person id if succeeded and -1 if not.

            if (string.IsNullOrEmpty(NationalNo))
            {
                throw new ArgumentException("NationalNo is required and cannot be empty.");
            }
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(SecondName) || string.IsNullOrEmpty(LastName))
            {
                throw new ArgumentException("First or Second or Last Name is required and cannot be empty.");
            }
            if (DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date Of Birth cannot be in the future.");
            }
            if (Gender != 0 && Gender != 1)
            {
                throw new ArgumentException("Invalid gender value. Please select Male or Female.");
            }
            if (string.IsNullOrEmpty(Address))
            {
                throw new ArgumentException("Address is required and cannot be empty.");
            }
            if (string.IsNullOrEmpty(Phone))
            {
                throw new ArgumentException("Phone is required and cannot be empty.");
            }
            if (NationalityCountryID < 0)
            {
                throw new ArgumentException("NationalityCountryID must be a valid positive integer.");
            }

            SqlConnection SqlConnection = new SqlConnection(Connection);

            SqlCommand command = new SqlCommand(InsertQuery, SqlConnection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@ThirdName", string.IsNullOrWhiteSpace(ThirdName) ? null : ThirdName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(Email) ? "" : Email);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
            command.Parameters.AddWithValue("@ImagePath", string.IsNullOrWhiteSpace(ImagePath) ? "" : ImagePath);

            try
            {
                SqlConnection.Open();

                object result = command.ExecuteScalar();
                //SqlConnection.Close();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    return insertedID;
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Error: " + ex.Message); 
            }
            finally
            {
                SqlConnection.Close();
            }

            return -1;
        }

        public static bool UpdatePerson(int ID, string NationalNo, string FirstName, string SecondName, string ThirdName,
             string LastName, short Gender, string Address, DateTime DateOfBirth, string Phone,
             string Email, int NationalityCountryID, string ImagePath)
        {

            int rowsAffected = 0;

            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand command = new SqlCommand(UpdateQuery, SqlConnection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@ThirdName", ThirdName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
            command.Parameters.AddWithValue("@ImagePath", ImagePath);

            try
            {
                SqlConnection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //Console.WriteLine("Error: " + ex.Message);
                return false;
            }
            finally
            {
                SqlConnection.Close();
            }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();

            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand command = new SqlCommand(AllPeopleQuery, SqlConnection);

            try
            {
                SqlConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception)
            {
                // error log here
            }
            finally
            {
                SqlConnection.Close();
            }

            return dt;
        }

        public static bool DeletePerson(int PersonID)
        {
            int rowsAffected = 0;

            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand command = new SqlCommand(DeletePersonQuery, SqlConnection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                SqlConnection.Open();

                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                SqlConnection.Close();
            }

            return (rowsAffected > 0);
        }

        public static bool IsPersonExist(int ID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM People WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", ID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        
  
    }
}
