using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Entities;

namespace BucketProject.DAL.Data.InterfacesRepo
{
    public interface IManagerRepo
    {
        List<UserEntity> GetAllUsers();

    }
}
