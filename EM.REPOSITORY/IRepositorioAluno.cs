using EM.DOMAIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EM.REPOSITORY
{
	public interface IRepositorioAluno<T> where T : IEntidade
	{
		public void Add(T obj);
		public void Remove(T obj);
		public void Update(T obj);
		public IEnumerable<T> GetAll();
		public IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
		public IEnumerable<Aluno> GetByContendoNoNome(string nome);
		public IEnumerable<Aluno> GetByMatricula(long matricula);

	}
}