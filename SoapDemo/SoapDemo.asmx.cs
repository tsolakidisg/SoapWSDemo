using Newtonsoft.Json;
using SoapDemo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace SoapDemo
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SoapDemo : System.Web.Services.WebService
    {
        [WebMethod]
        public DataTable GetOrderStatusById(int Id)
        {
            // Connection to Local MS SQL Server -> OrdersDB
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OrdersDB;Trusted_Connection=True;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Id, CustomerName, Fees, OrderStatus FROM Orders WHERE Id = " + @Id, connection);
            SqlDataAdapter sda = new SqlDataAdapter();

            cmd.Connection = connection;
            sda.SelectCommand = cmd;

            // Create a DataTable Object to store the select statemenent response
            DataTable dt1 = new DataTable();
            dt1.TableName = "Order";
            sda.Fill(dt1);

            return dt1;
        }

        [WebMethod]
        public XmlElement GetOrderStatus(int Id)
        {
            // Connection to Local MS SQL Server -> OrdersDB
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OrdersDB;Trusted_Connection=True;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Id, Fees, OrderStatus FROM Orders WHERE Id = " + @Id, connection);
            SqlDataAdapter sda = new SqlDataAdapter();

            cmd.Connection = connection;
            sda.SelectCommand = cmd;

            // Create a DataSet Object to store the select statemenent response
            DataSet ds = new DataSet();
            sda.Fill(ds);

            // Create an XML object that stores the DataSet's data from the select statement response
            System.Xml.XmlDataDocument xdd = new System.Xml.XmlDataDocument(ds);
            System.Xml.XmlElement docElement = xdd.DocumentElement;

            return docElement;
        }


        [WebMethod]
        public string UpdateDBRecords(int Id, string CustomerName)
        {
            // Connection to Local MS SQL Server -> OrdersDB
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OrdersDB;Trusted_Connection=True;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand updateCommand = connection.CreateCommand();
            SqlTransaction transaction;

            string returnMessage;

            // Use the connection and lock the data for editing
            using (transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead, "UpdateTransactionScript"))
            {
                updateCommand.Connection = connection;
                updateCommand.Transaction = transaction;

                try
                {
                    int numRows;
                    updateCommand.CommandText = "UPDATE Orders SET CustomerName = '" + CustomerName + "' WHERE Id = " + Id;
                    numRows = updateCommand.ExecuteNonQuery();
                    transaction.Commit();
                    returnMessage = numRows.ToString() + " Record(s) updated to database";
                }
                catch (Exception e)
                {
                    try
                    {
                        transaction.Rollback("UpdateTransactionScript");
                    }
                    catch (SqlException ex)
                    {
                        if (transaction.Connection != null)
                        {
                            returnMessage = "An exception of type " + ex.GetType() + " was encountered while attempting to roll back the transaction.";
                        }
                    }
                    returnMessage = "An exception of type " + e.GetType() + " was encountered while updating the data. No record was updated.";
                }
                finally
                {
                    connection.Close();
                }
            }

            return returnMessage;
        }

    }
}
