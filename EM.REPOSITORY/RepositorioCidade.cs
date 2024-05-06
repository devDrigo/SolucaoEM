using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.DOMAIN;
using System.Data.Common;
using System.Linq.Expressions;
using FirebirdSql.Data.FirebirdClient;


namespace EM.REPOSITORY
{
	public class RepositorioCidade : RepositorioAbstrato<CidadeModel>, IRepositorioCidade<CidadeModel>
	{

		private readonly FbConnection conn;

		public RepositorioCidade()
		{
			conn = BDConnect.GetConexao();
		}

		public override void Add(CidadeModel cidade)
		{
			using DbConnection cn = BDConnect.GetConexao();
			using DbCommand cmd = cn.CreateCommand();

			cmd.CommandText = "INSERT INTO CIDADES (NOME, UF) " +
								 "VALUES (@Nome, @UF)";

			cmd.Parameters.CreateParameter("@Nome", cidade.Nome!);
			cmd.Parameters.CreateParameter("@UF", cidade.UF!);

			cmd.ExecuteNonQuery();
		}

		public override void Remove(CidadeModel cidade)
		{
			using DbConnection cn = BDConnect.GetConexao();
			using DbCommand cmd = cn.CreateCommand();

			cmd.CommandText = "DELETE FROM CIDADES WHERE ID_CIDADE = @ID_CIDADE;";

			try
			{
				cmd.Parameters.CreateParameter("@ID_CIDADE", cidade.Id_cidade);

				cmd.ExecuteNonQuery();
				Console.WriteLine("Cidade Removida com Sucesso!");
			}
			catch (Exception erro)
			{
				Console.WriteLine($"Erro ao deletar Cidade, detalhe do erro {erro}");
			}
		}

		public override void Update(CidadeModel cidade)
		{
			using DbConnection cn = BDConnect.GetConexao();
			using DbCommand cmd = cn.CreateCommand();

			cmd.CommandText = "UPDATE Cidades SET Nome = @Nome, UF = @UF WHERE Id_Cidade = @Id_Cidade";


			cmd.Parameters.CreateParameter("@Nome", cidade.Nome!.ToUpper());
			cmd.Parameters.CreateParameter("@UF", cidade.UF!.ToUpper());
			cmd.Parameters.CreateParameter("@Id_Cidade", cidade.Id_cidade);
			cmd.ExecuteNonQuery();

		}

		public override IEnumerable<CidadeModel> Get(Expression<Func<CidadeModel, bool>> predicate)
		{
			return GetAll().Where(predicate.Compile());
		}

		public override IEnumerable<CidadeModel> GetAll()
		{

			using DbConnection cn = BDConnect.GetConexao();
			using DbCommand cmd = cn.CreateCommand();

			cmd.CommandText = @"SELECT * FROM CIDADES ORDER BY CIDADES.UF, CIDADES.NOME";

			List<CidadeModel> cidades = new List<CidadeModel>();


			DbDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				CidadeModel cidade = new CidadeModel();
				cidade.Id_cidade = Convert.ToInt32(reader["ID_CIDADE"]);
				cidade.Nome = reader["nome"].ToString();
				cidade.UF = reader["UF"].ToString();
				cidades.Add(cidade);
			}
			reader.Close();

			return cidades;
		}

	}



}