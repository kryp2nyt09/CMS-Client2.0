using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CMS2.Common.Enums;
using CMS2.DataAccess.Interfaces;
using CMS2.Entities;

namespace CMS2.DataAccess
{
    // use internal since only the UnitOfWork will access this class
    public class CmsRepository<TEntity> : ICmsRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private CmsContext _context = null;
        private DbSet<TEntity> _dbSet = null;

        public CmsRepository()
        {
            this._context = new CmsContext();
            this._dbSet = _context.Set<TEntity>();
        }

        public CmsRepository(CmsContext context)
        {
            this._context = context;
            this._dbSet = _context.Set<TEntity>();
        }

        protected DbSet<TEntity> dBSet
        {
            get { return _dbSet ?? (_dbSet = _context.Set<TEntity>()); }
        }

        private IQueryable<TEntity> GetEntities()
        {
            IQueryable<TEntity> result = _dbSet.AsQueryable();
            if (Includes != null)
            {
                foreach (var include in Includes)
                {
                    result = result.Include(include);
                }
            }
            return result;
        }

        public Expression<Func<TEntity, object>>[] Includes { get; set; }

        // returns all records. includes inactive and deleted
        public List<TEntity> GetAll()
        {
            return GetEntities().ToList();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await GetEntities().ToListAsync();
        }

        // returns all active records only
        public List<TEntity> FilterActive()
        {
            return GetEntities().Where(x => x.RecordStatus == (int)RecordStatus.Active).ToList();
        }

        public async Task<List<TEntity>> FilterActiveAsync()
        {
            return await GetEntities().Where(x => x.RecordStatus == (int)RecordStatus.Active).ToListAsync();
        }

        // parameter: filter
        // returns filtered records by field and value
        public List<TEntity> FilterBy(Expression<Func<TEntity, bool>> filter)
        {
            return GetEntities().Where(filter).ToList();
        }

        public async Task<List<TEntity>> FilterByAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await GetEntities().Where(filter).ToListAsync();
        }

        // parameter: filter
        // returns active filtered records by field and value
        public List<TEntity> FilterActiveBy(Expression<Func<TEntity, bool>> filter)
        {
            return GetEntities().Where(x => x.RecordStatus == (int)RecordStatus.Active).Where(filter).ToList();
        }

        public async Task<List<TEntity>> FilterActiveByAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await GetEntities().Where(x => x.RecordStatus == (int)RecordStatus.Active).Where(filter).ToListAsync();
        }

        public bool IsExist(Expression<Func<TEntity, bool>> filter)
        {
            if (_dbSet.Where(filter).Any())
                return true;
            return false;
        }

        public TEntity GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public TEntity GetById(long id)
        {
            return _dbSet.Find(id);
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Create(TEntity entity)
        {
            try
            {
                _dbSet.Add(entity);
            }
            catch (Exception ex)
            {
            }
            
        }

        public void Update(TEntity entity)
        {
            string primaryKey = GetPrimaryKey(entity);

            TEntity model = _dbSet.Find(GetEntityId(entity));
            string propertyName = "";
            foreach (PropertyInfo prop in model.GetType().GetProperties())
            {
                propertyName = prop.Name;
                var propertyAttributes = entity.GetType().GetProperty(propertyName).GetCustomAttributes(false);
                bool isNotMapped = false;
                foreach (var att in propertyAttributes)
                {
                    if (att.GetType() == typeof (NotMappedAttribute))
                    {
                        isNotMapped = true;
                        break;
                    }
                }
                var propertyValue = entity.GetType().GetProperty(propertyName).GetValue(entity);
                if (propertyName.Equals(primaryKey) || // specific properties to be excluded
                    propertyName.Contains("Created") ||
                    propertyName.Equals("Record_Status") ||
                    propertyName.Equals("RecordStatusString") || 
                    propertyName.Equals("FullName") ||
                    isNotMapped
                    ) 
                {
                }
                else
                {
                    prop.SetValue(model, propertyValue);
                }
            }
            _context.Entry(model).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            TEntity entity = _dbSet.Find(id);
            entity.RecordStatus = (int)RecordStatus.Deleted;
            Update(entity);
        }

        public void Delete(int id)
        {
            TEntity entity = _dbSet.Find(id);
            entity.RecordStatus = (int)RecordStatus.Deleted;
            Update(entity);
        }

        public void Delete(long id)
        {
            TEntity entity = _dbSet.Find(id);
            entity.RecordStatus = (int)RecordStatus.Deleted;
            Update(entity);
        }

        public void DeletePhysically(Guid id)
        {
            TEntity entity = _dbSet.Find(id);
            _dbSet.Remove(entity);
        }

        public void DeletePhysically(int id)
        {
            TEntity entity = _dbSet.Find(id);
            _dbSet.Remove(entity);
        }

        public void DeletePhysically(long id)
        {
            TEntity entity = _dbSet.Find(id);
            _dbSet.Remove(entity);
        }

        private dynamic GetEntityId(TEntity entity)
        {
            string primaryKey = GetPrimaryKey(entity);
            Type _type =
                entity.GetType().GetProperties().Where(x => x.Name.Equals(primaryKey)).FirstOrDefault().PropertyType;

            if (_type == typeof(Int32))
            {
                return Convert.ToInt32(entity.GetType().GetProperties().Where(x => x.Name.Equals(primaryKey)).FirstOrDefault().GetValue(entity).ToString());
            }

            if (_type == typeof(Int64))
            {
                return Convert.ToInt32(entity.GetType().GetProperties().Where(x => x.Name.Equals(primaryKey)).FirstOrDefault().GetValue(entity).ToString());
            }

            if (_type == typeof(Guid))
            {
                return new Guid(entity.GetType().GetProperties().Where(x => x.Name.Equals(primaryKey)).FirstOrDefault().GetValue(entity).ToString());
            }

            return null;
        }

        private string GetPrimaryKey(TEntity entity)
        {
            var key =
                entity.GetType()
                    .GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttributes(typeof(KeyAttribute)).Any());

            return key.Name;
        }
    }
}
