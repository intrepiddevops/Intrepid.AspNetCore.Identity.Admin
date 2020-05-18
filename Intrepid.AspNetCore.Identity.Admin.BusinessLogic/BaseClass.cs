using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.BusinessLogic
{
    
    public abstract class BaseClass
    {
        public UserManager<IdentityUser> Manager { get; private set; }
        public IdentityDbContext Context { get; }
        public IMapper Mapper { get; }
        public ILogger Logger { get; }
        public BaseClass(UserManager<IdentityUser> manager, IdentityDbContext context, IMapper mapper, ILogger logger)
        {
            Manager = manager;
            Context = context;
            Mapper = mapper;
            Logger = logger;
        }
    }
}
