using CLED.BlazorAuthentication.Models;
using MySql.Data.MySqlClient;

namespace CLED.BlazorAuthentication.Services
{
	public class ProductService
	{
		private readonly string _connectionString;
		public ProductService(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection")!;
		}

		public async Task<IEnumerable<Product>> GetAll()
		{
			var list = new List<Product>();

			using var connection = new MySqlConnection(_connectionString);
			await connection.OpenAsync();

			string query = @"
            SELECT 
                id,
                name,
                price,
                createdby,createddate,modifiedby,modifieddate,isdeleted,deletedby,
				deleteddate
            FROM Products
WHERE isDeleted = 0;
            ";
			using var command = connection.CreateCommand();
			command.CommandText = query;

			using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				var Product = new Product();
				list.Add(Product);

				Product.Id = (int)reader["id"];
				Product.Name = reader.GetString(reader.GetOrdinal("name")); // restituisce la posizione della colonna del recordset.
				Product.Price = (decimal)reader["price"]; ;
				Product.CreatedBy = (string)reader["createdBy"]; ;
				Product.CreatedDate = (DateTime)reader["createdDate"];
				Product.ModifiedBy = reader["modifiedby"] as string;
				Product.ModifiedDate = (DateTime?)(reader["modifiedDate"] != DBNull.Value ? reader["modifieddate"] : null);
				Product.IsDeleted = Convert.ToBoolean(reader["isdeleted"]);
				Product.DeletedBy = reader["deletedby"] as string;
				Product.DeletedDate = (DateTime?)(reader["deletedDate"] != DBNull.Value ? reader["deletedDate"] : null);
			}

			return list;
		}

		public async Task<Product?> GetById(int ProductId)
		{
			using var connection = new MySqlConnection(_connectionString);
			await connection.OpenAsync();

			string query = @"
            SELECT 
                id,
                name,
                price,
                createdby,
            	createddate,
            	modifiedby,
            	modifieddate,
            	isdeleted,
            	deletedby,
            	deleteddate
            FROM Products
            WHERE id = @id;
            ";
			using var command = connection.CreateCommand();
			command.CommandText = query;
			command.Parameters.AddWithValue("id", ProductId);

			using var reader = await command.ExecuteReaderAsync();
			if (await reader.ReadAsync())
			{
				var Product = new Product();
				Product.Id = (int)reader["id"];
				Product.Name = reader.GetString(reader.GetOrdinal("name")); // restituisce la posizione della colonna del recordset.
				Product.Price = (decimal)reader["price"]; ;
				Product.CreatedBy = (string)reader["createdBy"]; ;
				Product.CreatedDate = (DateTime)reader["createdDate"];
				Product.ModifiedBy = reader["modifiedby"] as string;
				Product.ModifiedDate = (DateTime?)(reader["modifiedDate"] != DBNull.Value ? reader["modifieddate"] : null);
				Product.IsDeleted = Convert.ToBoolean(reader["isdeleted"]);
				Product.DeletedBy = reader["deletedby"] as string;
				Product.DeletedDate = (DateTime?)(reader["deletedDate"] != DBNull.Value ? reader["deletedDate"] : null);
				return Product;
			}

			return null;
		}

		public async Task Insert(Product Product)
		{
			using var connection = new MySqlConnection(_connectionString);
			await connection.OpenAsync();

			string query = @"
            INSERT INTO Products (
            name,
            price,
            createdby,
            createddate,
            isdeleted)
            VALUES (
            @name,
            @price,
            @createdby,
            @createddate,
            @isdeleted);";
			using var command = connection.CreateCommand();
			command.CommandText = query;
			command.Parameters.AddWithValue("name", Product.Name);
			command.Parameters.AddWithValue("price", Product.Price);
			command.Parameters.AddWithValue("CreatedDate", Product.CreatedDate);
			command.Parameters.AddWithValue("CreatedBy", Product.CreatedBy ?? "-1");
			command.Parameters.AddWithValue("isDeleted", Product.IsDeleted);
			//command.Parameters.AddWithValue("modifieddate", Product.ModifiedDate ?? (object)DBNull.Value);
			//command.Parameters.AddWithValue("ModifiedBy", Product.ModifiedBy ?? (object)DBNull.Value);
			//command.Parameters.AddWithValue("DeletedDate", Product.DeletedDate ?? (object)DBNull.Value);
			//command.Parameters.AddWithValue("DeletedBy", Product.DeletedBy ?? (object)DBNull.Value);

			await command.ExecuteNonQueryAsync();
		}

		public async Task Update(Product Product)
		{
			using var connection = new MySqlConnection(_connectionString);
			await connection.OpenAsync();

			string query = @"
            UPDATE Products
            SET 
                name = @name,
				price = @price,
				modifiedby = @modifiedby,
				modifieddate = @modifieddate,
				isdeleted = @isdeleted,
				deletedby = @deletedby,
				deleteddate = @deleteddate
            WHERE id = @id;
            ";
			using var command = connection.CreateCommand();
			command.CommandText = query;
			command.Parameters.AddWithValue("Id", Product.Id);
			command.Parameters.AddWithValue("name", Product.Name);
			command.Parameters.AddWithValue("price", Product.Price);
			//command.Parameters.AddWithValue("CreatedDate", Product.CreatedDate);
			//command.Parameters.AddWithValue("CreatedBy", Product.CreatedBy ?? "-1");
			command.Parameters.AddWithValue("modifieddate", Product.ModifiedDate ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("ModifiedBy", Product.ModifiedBy ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("isDeleted", Product.IsDeleted);
			command.Parameters.AddWithValue("DeletedDate", Product.DeletedDate ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("DeletedBy", Product.DeletedBy ?? (object)DBNull.Value);

			await command.ExecuteNonQueryAsync();
		}

		public async Task Delete(int ProductId)
		{
			using var connection = new MySqlConnection(_connectionString);
			await connection.OpenAsync();

			string query = """
                DELETE FROM Products
                WHERE id = @id;
                """;
			using var command = new MySqlCommand(query, connection);
			command.Parameters.AddWithValue("id", ProductId);

			await command.ExecuteNonQueryAsync();
		}

		public async Task<long> Count()
		{
			using var connection = new MySqlConnection(_connectionString);
			await connection.OpenAsync();

			string query = "SELECT COUNT(*) FROM Products;";
			using var command = connection.CreateCommand();
			command.CommandText = query;

			return (long)(await command.ExecuteScalarAsync() ?? 0);
		}
	}
}
