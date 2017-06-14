using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using NAlex.DataModel.Entities;
using NAlex.Selling.DTO.Classes;

namespace NAlex.Selling.DAL.Repositories
{
    public class ManagersRepository: DtoRepository<Manager, ManagerDTO, int>
    {
        public ManagersRepository(DbContext context): base(context)
        {
        }
    }
}
