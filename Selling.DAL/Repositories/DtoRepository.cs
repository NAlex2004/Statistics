using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using AutoMapper.Mappers;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace NAlex.Selling.DAL.Repositories
{
    public abstract class DtoRepository<TEntity, TDto, TKey> : IRepository<TDto, TKey>
        where TDto : class
        where TEntity: class
    {
        protected DbContext _context;        

        public DtoRepository(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            
            Mapper.CreateMap<TEntity, TDto>();
            Mapper.CreateMap<TDto, TEntity>();                        
        }

        public virtual TDto Get(TKey Id)
        {
            return Mapper.Map<TEntity, TDto>(_context.Set<TEntity>().Find(Id));
        }

        public virtual IEnumerable<TDto> GetAll()
        {
            
            return _context.Set<TEntity>().UseAsDataSource().For<TDto>().ToArray();
        }

        public virtual IQueryable<TDto> GetAsQueryable(Expression<Func<TDto, bool>> condition,
            Func<IQueryable<TDto>, IOrderedQueryable<TDto>> orderBy = null)
        {            
            var q = _context.Set<TEntity>().UseAsDataSource().For<TDto>().Where(condition);
            if (orderBy != null)
                return orderBy(q); 
            return q;
        }

        public virtual IEnumerable<TDto> Get(Func<TDto, bool> condition,
            Func<IEnumerable<TDto>, IOrderedEnumerable<TDto>> orderBy = null)
        {
            var q = _context.Set<TEntity>().UseAsDataSource().For<TDto>().Where(condition);

            if (orderBy != null)
                return orderBy(q).ToArray();

            return q.ToArray();
        }

        public virtual TDto Add(TDto entity)
        {
            if (entity == null)
                return null;
            if (_context.Set<TEntity>().Local.Any(e => Mapper.Map<TDto>(e).Equals(entity)))
                return entity;            

            TEntity newEntity = Mapper.Map<TEntity>(entity);

            return Mapper.Map<TDto>(_context.Set<TEntity>().Add(newEntity));
        }

        public virtual void Remove(TDto entity)
        {
            if (entity == null)
                return;

            TEntity local = _context.Set<TEntity>().Local.FirstOrDefault(e => Mapper.Map<TDto>(e).Equals(entity));
            if (local != null)
            {
                if (_context.Entry<TEntity>(local).State == EntityState.Added)
                    _context.Entry<TEntity>(local).State = EntityState.Detached;
                else
                    _context.Set<TEntity>().Remove(local);

                return;
            }

            TEntity newEntity = Mapper.Map<TEntity>(entity);
            TEntity attached = _context.Set<TEntity>().Attach(newEntity);
            _context.Entry<TEntity>(newEntity).State = EntityState.Deleted;
        }

        public virtual void Remove(Func<TDto, bool> condition)
        {
            Func<TEntity, bool> fun = Mapper.Map<Func<TEntity, bool>>(condition);
            _context.Set<TEntity>().Local.Where(fun)
                .ToList()
                .ForEach(e => _context.Entry<TEntity>(e).State = EntityState.Detached);
            _context.Set<TEntity>().Where(fun)
                .ToList()
                .ForEach(e => _context.Set<TEntity>().Remove(e));
        }

        public virtual void Remove(TKey Id)
        {
            TDto entity = Get(Id);
            if (entity != null)
                Remove(entity);
        }

        public virtual bool Update(TDto entity)
        {
            if (entity == null)
                return false;
            
            TEntity newEntity = Mapper.Map<TEntity>(entity);
            
            var props = newEntity.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());
            if (props == null)
                return false;
            
            TEntity existing = _context.Set<TEntity>().Find(props.GetValue(newEntity, null));
            if (existing == null)
                return false;

            _context.Entry<TEntity>(existing).CurrentValues.SetValues(newEntity);
            _context.Entry<TEntity>(existing).State = EntityState.Modified;

            return true;
        }

        public void AddRange(IEnumerable<TDto> range)
        {
            if (range == null)
                return;

            foreach (TDto item in range)
                Add(item);
        }        
    }


}
