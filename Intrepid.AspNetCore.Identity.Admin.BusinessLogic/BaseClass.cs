using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.DbContexts;
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
        
        public ApplicationDbContext Context { get; }
        public IMapper Mapper { get; }
        public ILogger Logger { get; }
        public BaseClass(ApplicationDbContext context, IMapper mapper, ILogger logger)
        {
            Context = context;
            Mapper = mapper;
            Logger = logger;
        }
    }
}
