using EM.DOMAIN;
using System.Linq.Expressions;
using System.Data.Common;

namespace EM.REPOSITORY
{
    public class RepositorioAluno : RepositorioAbstrato<Aluno>, IRepositorioAluno<Aluno>
    {
        public override void Add(Aluno aluno)
        {
            try
            {
                using DbConnection cn = BDConnect.GetConexao();
                using DbCommand cmd = cn.CreateCommand();

                cmd.CommandText = "INSERT INTO ALUNOS (MATRICULA, NOME, CIDADE_ID, CPF, DATANASCIMENTO, SEXO) " +
                                     "VALUES (@MATRICULA, @Nome, @Cidade_id, @CPF, @DataNascimento, @Sexo)";

                cmd.Parameters.CreateParameter("@MATRICULA", long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                cmd.Parameters.CreateParameter("@Nome", aluno.Nome);
                cmd.Parameters.CreateParameter("@Cidade_id", aluno.Cidade.Id_cidade);
				cmd.Parameters.CreateParameter("@CPF", aluno.CPF != null ? aluno.CPF : DBNull.Value);
				cmd.Parameters.CreateParameter("@DataNascimento", aluno.DataNascimento!);
                cmd.Parameters.CreateParameter("@Sexo", aluno.Sexo);

                cmd.ExecuteNonQuery();
            }
            catch (Exception erro)
            {
                Console.WriteLine($"Erro ao adicionar Aluno, detalhe do erro {erro}");
            }
        }


        public override void Remove(Aluno aluno)
        {
            try
            {
                using DbConnection cn = BDConnect.GetConexao();
                using DbCommand cmd = cn.CreateCommand();

                cmd.CommandText = "DELETE FROM ALUNOS WHERE MATRICULA = @MATRICULA ;";
                cmd.Parameters.CreateParameter("@MATRICULA", aluno.Matricula);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Aluno Removido com Sucesso!");
            }
            catch (Exception erro)
            {
                Console.WriteLine($"Erro ao deletar um Aluno, detalhe do erro {erro}");
            }
        }


        public override void Update(Aluno aluno)
        {
            try
            {
                using DbConnection cn = BDConnect.GetConexao();
                using DbCommand cmd = cn.CreateCommand();

                cmd.CommandText = @"UPDATE Alunos SET Nome = @Nome, CPF = @CPF, DataNascimento = @DataNascimento, Sexo = @Sexo, Cidade_ID = @Cidade_ID  WHERE Matricula = @Matricula";

                cmd.Parameters.CreateParameter("@Matricula", aluno.Matricula);
                cmd.Parameters.CreateParameter("@Nome", aluno.Nome);
                cmd.Parameters.CreateParameter("@Cidade_id", aluno.Cidade!.Id_cidade);
				cmd.Parameters.CreateParameter("@CPF", aluno.CPF != null ? aluno.CPF : DBNull.Value);
				cmd.Parameters.CreateParameter("@DataNascimento", aluno.DataNascimento!);
                cmd.Parameters.CreateParameter("@Sexo", aluno.Sexo!);

                cmd.ExecuteNonQuery();
            }
            catch (Exception erro)
            {
                Console.WriteLine($"Erro ao atualizar Aluno, detalhe do erro {erro}");
            }
        }

        public override IEnumerable<Aluno> Get(Expression<Func<Aluno, bool>> predicate)
        {
            try
            {
                return GetAll().Where(predicate.Compile());
            }
            catch (Exception erro)
            {
                Console.WriteLine($"Erro ao obter Alunos, detalhe do erro {erro}");
                return Enumerable.Empty<Aluno>();
            }
        }

        public override IEnumerable<Aluno> GetAll()
        {
            try
            {
                using DbConnection cn = BDConnect.GetConexao();
                using DbCommand cmd = cn.CreateCommand();

                // Inclui ID_cidade na consulta
                cmd.CommandText = @"SELECT A.matricula, A.nome, A.sexo, A.dataNascimento, A.CPF, 
                               C.nome AS nomeCidade, C.UF as UFCidade, C.ID_cidade 
												 FROM Alunos A
								INNER JOIN Cidades C ON A.cidade_ID = C.ID_cidade 
											ORDER BY A.matricula ASC";

                List<Aluno> alunos = new List<Aluno>();

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Aluno aluno = new Aluno
                    {
                        Matricula = Convert.ToInt64(reader["matricula"]),
                        Nome = reader["nome"].ToString()!,
                        CPF = reader["CPF"].ToString(),
                        DataNascimento = Convert.ToDateTime(reader["datanascimento"]),
                        Sexo = (SexoEnum) Convert.ToInt32(reader["sexo"]),
                        Cidade = new CidadeModel
                        {
                            Nome = reader["NomeCidade"].ToString(),
                            UF = reader["UFCidade"].ToString(),
                            Id_cidade = Convert.ToInt32(reader["ID_cidade"])
                        }
                    };

                    alunos.Add(aluno);
                }
                reader.Close();
                return alunos;
            }
            catch (Exception erro)
            {
                Console.WriteLine($"Erro ao obter todos os Alunos, detalhe do erro {erro}");
                return Enumerable.Empty<Aluno>();
            }
        }


        public IEnumerable<Aluno> GetByMatricula(long matricula)
        {
            try
            {
                using DbConnection cn = BDConnect.GetConexao();
                using DbCommand cmd = cn.CreateCommand();

                cmd.CommandText = @"SELECT A.matricula, A.nome, A.sexo, A.dataNascimento, A.CPF, C.nome AS nomeCidade, C.UF as UFCidade FROM Alunos A
																				INNER JOIN 
												Cidades C ON A.cidade_ID = C.ID_cidade WHERE A.matricula = @matricula 
																		order by A.matricula asc";

                List<Aluno> alunos = new List<Aluno>();

                // Adicione esta linha para declarar o parâmetro @matricula
                cmd.Parameters.CreateParameter("@matricula", matricula);

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Aluno aluno = new Aluno();
                    aluno.Matricula = Convert.ToInt64(reader["matricula"]);
                    aluno.Nome = reader["nome"].ToString()!;
                    aluno.CPF = reader["CPF"].ToString();
                    aluno.DataNascimento = Convert.ToDateTime(reader["datanascimento"]);
                    aluno.Sexo = (SexoEnum)reader.GetInt32(reader.GetOrdinal("SEXO"));
                    // Preencher as informações da cidade associada ao aluno
                    aluno.Cidade = new CidadeModel();
                    aluno.Cidade.Nome = reader["NomeCidade"].ToString();
                    aluno.Cidade.UF = reader["UFCidade"].ToString();
                    alunos.Add(aluno);
                }
                reader.Close();

                return alunos;
            }
            catch (Exception erro)
            {
                Console.WriteLine($"Erro ao obter Alunos por matrícula, detalhe do erro {erro}");
                return Enumerable.Empty<Aluno>();
            }
        }

        public IEnumerable<Aluno> GetByContendoNoNome(string nome)
        {
            try
            {
                using DbConnection cn = BDConnect.GetConexao();
                using DbCommand cmd = cn.CreateCommand();

                cmd.CommandText = @"SELECT A.matricula, A.nome, A.sexo, A.dataNascimento, A.CPF, C.nome AS nomeCidade, C.UF as UFCidade FROM Alunos A
																					INNER JOIN 
												Cidades C ON A.cidade_ID = C.ID_cidade WHERE A.nome LIKE '%' || @nomeParcial || '%' 
																			order by A.matricula asc";

                List<Aluno> alunos = new List<Aluno>();

                //declarar o parâmetro @nomeParcial
                cmd.Parameters.CreateParameter("@nomeParcial", nome);

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Aluno aluno = new Aluno();
                    aluno.Matricula = Convert.ToInt64(reader["matricula"]);
                    aluno.Nome = reader["nome"].ToString()!;
                    aluno.CPF = reader["CPF"].ToString();
                    aluno.DataNascimento = Convert.ToDateTime(reader["datanascimento"]);
                    aluno.Sexo = (SexoEnum)reader.GetInt32(reader.GetOrdinal("SEXO"));
                    // Preencher as informações da cidade associada ao aluno
                    aluno.Cidade = new CidadeModel();
                    aluno.Cidade.Nome = reader["NomeCidade"].ToString();
                    aluno.Cidade.UF = reader["UFCidade"].ToString();
                    alunos.Add(aluno);
                }
                reader.Close();
                return alunos;
            }
            catch (Exception erro)
            {
                Console.WriteLine($"Erro ao obter Alunos contendo nome, detalhe do erro {erro}");
                return Enumerable.Empty<Aluno>();
            }
        }
    }
}