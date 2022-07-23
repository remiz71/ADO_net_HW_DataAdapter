using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using static System.Console;
using System.Net.Http.Headers;

namespace ADO_net_HW_DataAdapter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter employeeAdapter = new SqlDataAdapter("SELECT * FROM [dbo].[Employee]", conn);
                SqlCommandBuilder empBuilder = new SqlCommandBuilder(employeeAdapter);
                DataSet _dataset = new DataSet();

                employeeAdapter.Fill(_dataset, "Employee");
                DataTable _empTable = _dataset.Tables["Employee"];
                tablePrint(_empTable);
                //insert

                _empTable.PrimaryKey = new DataColumn[] { _empTable.Columns["EmployeeID"] };
                employeeAdapter.InsertCommand = new SqlCommand("stp_EmployeeAdd", conn);
                employeeAdapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                employeeAdapter.InsertCommand.Parameters.AddWithValue("@FirstName", "Ivam");
                employeeAdapter.InsertCommand.Parameters.AddWithValue("@LastName", "Petrov");
                employeeAdapter.InsertCommand.Parameters.AddWithValue("@BirthDate", "1910-01-23");
                employeeAdapter.InsertCommand.Parameters.AddWithValue("@PositionID", 2);
                employeeAdapter.InsertCommand.Parameters.Add("EmployeeID", SqlDbType.Int).Direction = ParameterDirection.Output;

                //employeeAdapter.InsertCommand.ExecuteNonQuery();
                _empTable.Clear();
                employeeAdapter.Fill(_dataset, "Employee");
                _empTable = _dataset.Tables["Employee"];
                Console.WriteLine("Таблица после инсерта");
                tablePrint(_empTable);

                //delete
                employeeAdapter.DeleteCommand = new SqlCommand("stp_EmployeeDelete_1", conn);
                employeeAdapter.DeleteCommand.CommandType = CommandType.StoredProcedure;
                employeeAdapter.DeleteCommand.Parameters.AddWithValue("EmployeeID", 1008);
                employeeAdapter.DeleteCommand.ExecuteNonQuery();
                updateDataSet(_empTable, employeeAdapter, _dataset);
                Console.WriteLine("Таблица после удаления");
                tablePrint(_empTable);

                //update
                employeeAdapter.UpdateCommand = new SqlCommand("stp_EmployeeUpdate", conn);
                employeeAdapter.UpdateCommand.CommandType = CommandType.StoredProcedure;
                employeeAdapter.UpdateCommand.Parameters.AddWithValue("@FirstName", "Semen");
                employeeAdapter.UpdateCommand.Parameters.AddWithValue("@LastName", "Semenovich");
                employeeAdapter.UpdateCommand.Parameters.AddWithValue("@BirthDate", "1950-01-23");
                employeeAdapter.UpdateCommand.Parameters.AddWithValue("@PositionID", 5);
                employeeAdapter.UpdateCommand.Parameters.AddWithValue("@EmployeeID", 1007);
                employeeAdapter.UpdateCommand.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                employeeAdapter.UpdateCommand.ExecuteNonQuery();
                Console.WriteLine("талблица послае апдейта");
                updateDataSet(_empTable, employeeAdapter, _dataset);
                tablePrint(_empTable);


            }
        }
        public static void tablePrint(DataTable epmTable)
        {
            foreach (DataRow dataRow in epmTable.Rows)
            {
                var _eID = dataRow["EmployeeID"];
                var _eName = dataRow["FirstName"] + " " + dataRow["LastName"];
                var _eBirth = dataRow["BirthDate"];
                var _ePos = dataRow["PositionID"];
                var _eSalary = dataRow["Salary"];
                WriteLine($"{_eID}\t{_eName}\t{DateTime.Parse(_eBirth.ToString()).ToShortDateString()}" +
                    $"\t{_ePos}\t{_eSalary}");

            }
        }

        public static void updateDataSet(DataTable empTable, SqlDataAdapter adapter, DataSet dataset)
        {
            empTable.Clear();
            adapter.Fill(dataset, "Employee");
            empTable = dataset.Tables["Employee"];
        }
    }
}
