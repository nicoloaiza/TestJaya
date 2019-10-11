using System;
using TestJaya.Data.DAL;
using TestJaya.Data.Models;

namespace TestJaya.Data.Repositories
{
    public class UserRepository : BaseStoredDataRepository<User, Guid>
    {
    }
}
