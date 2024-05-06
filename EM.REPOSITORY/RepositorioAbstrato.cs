using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EM.DOMAIN;


using FirebirdSql.Data.FirebirdClient;

namespace EM.REPOSITORY
{
	public abstract class RepositorioAbstrato<T>
	{
		public abstract void Add(T objeto);
		public abstract void Remove(T objeto);
		public abstract void Update(T objeto);
		public abstract IEnumerable<T> GetAll();
		public abstract IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
	}
}