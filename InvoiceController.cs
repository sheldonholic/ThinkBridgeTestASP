using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;

namespace BuggyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private const string DbFileName = "data.db";
        private const string InitSqlFile = "init.sql";

        // GET api/invoice?id=1
        [HttpGet]
        public IActionResult GetInvoice([FromQuery] int id = 1)
        {
            EnsureDatabase();

            var items = new List<ItemDto>();
            string? customerName = null;

            using (var conn = new SqliteConnection($"Data Source={DbFileName}"))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT CustomerName FROM Invoices WHERE InvoiceID=@id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    var result = cmd.ExecuteScalar();
                    if (result != null) customerName = result.ToString();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name, Price FROM InvoiceItems WHERE InvoiceID=@id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            double price;
                            try
                            {
                                price = reader.IsDBNull(1) ? 0.0 : reader.GetDouble(1);
                            }
                            catch
                            {
                                price = reader.IsDBNull(1) ? 0.0 : (double)reader.GetDecimal(1);
                            }

                            items.Add(new ItemDto { name = name, price = price });
                        }
                    }
                }
            }

            if (items.Count == 0)
                return NotFound("No invoice found");

            return Ok(new
            {
                invoiceId = id,
                customerName,
                items
            });
        }

        private static void EnsureDatabase()
        {
            if (System.IO.File.Exists(DbFileName)) return;

            if (!System.IO.File.Exists(InitSqlFile))
            {
                using (var conn = new SqliteConnection($"Data Source={DbFileName}"))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Invoices (
    InvoiceID INTEGER PRIMARY KEY,
    CustomerName TEXT
);
CREATE TABLE IF NOT EXISTS InvoiceItems (
    ItemID INTEGER PRIMARY KEY,
    InvoiceID INTEGER,
    Name TEXT,
    Price REAL,
    FOREIGN KEY (InvoiceID) REFERENCES Invoices(InvoiceID)
);
INSERT INTO Invoices (InvoiceID, CustomerName) VALUES (1, 'John Doe');
INSERT INTO InvoiceItems (ItemID, InvoiceID, Name, Price) VALUES (1, 1, 'Widget A', 19.99);
";
                        cmd.ExecuteNonQuery();
                    }
                }
                return;
            }

            var sql = System.IO.File.ReadAllText(InitSqlFile);
            using (var conn = new SqliteConnection($"Data Source={DbFileName}"))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        var parts = sql.Split(';');
                        foreach (var p in parts)
                        {
                            var trimmed = p.Trim();
                            if (string.IsNullOrEmpty(trimmed)) continue;
                            cmd.CommandText = trimmed + ";";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public class ItemDto
        {
            public string name { get; set; } = string.Empty;
            public double price { get; set; }
        }
    }
}
