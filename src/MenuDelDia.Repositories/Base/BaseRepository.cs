using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using MenuDelDia.Entities;

namespace MenuDelDia.Repository.Base
{
    public abstract class BaseRepository<T> : IRepository<T> where T : EntityBase
    {
        private readonly DbContext _context;
        private readonly IDbSet<T> _dbset;
        private readonly Database _db;
        

        protected BaseRepository(DbContext context)
        {
            _context = context;            
            _dbset = context.Set<T>();
            _db = context.Database;
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.ProxyCreationEnabled = false;
        }

        public bool ContextTrackChanges
        {
            get { return _context.Configuration.AutoDetectChangesEnabled; }
            set { _context.Configuration.AutoDetectChangesEnabled = value; }
        }

        public IQueryable<T> All(Expression<Func<T, bool>> includeFilters = null,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbset.AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (includeFilters == null)
                return query;

            return query.Where(includeFilters);
        }

        public IQueryable<T> AllNoTracking(Expression<Func<T, bool>> includeFilters = null,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbset.AsQueryable();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (includeFilters == null)
                return query.AsQueryable().AsNoTracking();

            return query.AsQueryable().AsNoTracking().Where(includeFilters);
        }


        public T GetById(Guid id, params Expression<Func<T, object>>[] properties)
        {
            var entity = _dbset.Find(id);

            if (entity != null)
            {
                foreach (var includeProperty in properties)
                {
                    bool cargado;
                    try
                    {
                        _context.Entry(entity).Collection(includeProperty.ToString().Split('.')[1]).Load();
                        cargado = true;
                    }
                    catch (Exception)
                    {
                        cargado = false;
                    }

                    if (!cargado)
                    {
                        _context.Entry(entity).Reference(includeProperty).Load();
                    }
                }
            }
            return entity;
        }

        public void Create(T entity)
        {
            //var auditable = entity as IAuditable;
            //if (auditable != null)
            //{
            //    auditable.CreationDate = DateTime.Now;
            //    auditable.CreationUser = TransactionContext.UserName;
            //    auditable.LastModificationDate = DateTime.Now;
            //    auditable.LastModificationUser = TransactionContext.UserName;
            //}
            entity.GenerateNewIdentity();
            _dbset.Add(entity);
        }

        public void Edit(T entity)
        {
            //If Auditable
            //var auditable = entity as IAuditable;
            //if (auditable != null)
            //{
            //    auditable.LastModificationDate = DateTime.Now;
            //    auditable.LastModificationUser = TransactionContext.UserName;
            //}

            _context.Entry(entity).State = EntityState.Modified;
        }


        public void Delete(Guid id)
        {
            var entity = GetById(id);
            _dbset.Remove(entity);
        }

        public void Delete(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        public void AddEntitiesNoRepository(object entity)
        {
            if (entity != null)
            {
                if (IsProxy(entity))
                    _context.Set(entity.GetType().BaseType).Add(entity);
                else
                    _context.Set(entity.GetType()).Add(entity);
            }
        }

        public void DeleteEntitiesNoRepository(object entity)
        {
            if (entity != null)
            {
                if (IsProxy(entity))
                    _context.Set(entity.GetType().BaseType).Remove(entity);
                else
                    _context.Set(entity.GetType()).Remove(entity);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Atach(T entity)
        {
            var state = _context.Entry(entity).State;

            if (state == EntityState.Detached)
                _dbset.Attach(entity);
        }

        public int ExecuteSQL(string sql, SqlParameter[] parameters)
        {
            return parameters == null
                ? _db.ExecuteSqlCommand(sql)
                : _db.ExecuteSqlCommand(sql, parameters.ToArray<object>());
        }

        private static bool IsProxy(object type)
        {
            return type != null && ObjectContext.GetObjectType(type.GetType()) != type.GetType();
        }
    }
}
