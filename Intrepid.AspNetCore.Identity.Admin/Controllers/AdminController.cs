﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Intrepid.AspNetCore.Identity.Admin.Models;
using System.Security.Claims;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic;
using AutoMapper;

namespace Intrepid.AspNetCore.Identity.Admin.Controllers
{
    [Authorize(Policy = "AdminManagerRole")]
    public class AdminController : BaseController
    {
        public RoleService RoleService { get; }
        public IMapper Mapper { get; }
        public IdentityService IdentityService { get; }

        public AdminController(RoleService roleService, IMapper mapepr, IdentityService identityService)
        {
            this.RoleService = roleService;
            this.Mapper = mapepr;
            this.IdentityService = identityService;
        }
        /// <summary>
        /// Dashboard
        /// </summary>
        /// <returns></returns>
        public async Task< IActionResult> Index()
        {
            var rolesOriginal = await RoleService.AllRoleInfo();
            var roles = rolesOriginal.ReturnObject.Select(x => Mapper.Map<RoleCountModel>(x)).ToList();
            var totoalCount = await this.IdentityService.UsersMetaData();
            DashboardViewModel vm = new DashboardViewModel()
            {
                TotalUsers = totoalCount.TotalNumberUsers,
                LockedUsers = totoalCount.TotlaLockedOut,
                PhoneNumberConfirmUsers = totoalCount.PhoneNumberConfirmed,
                EmailUnconfirmedUsers = totoalCount.TotalEmailNotConfirm,
                RoleCounts = roles,
                //{ 
                //    new RoleCountModel() 
                //    { 
                //        RoleId = "1",
                //        Name = "Administrator", 
                //        Count = 3 
                //    },
                //    new RoleCountModel()
                //    {
                //        RoleId = "2",
                //        Name = "Super User",
                //        Count = 10
                //    },
                //    new RoleCountModel()
                //    {
                //        RoleId = "2",
                //        Name = "Data Clerk",
                //        Count = 500
                //    },
                //}
            };

            return View(vm);
        }

        public IActionResult Users()
        {
            UsersViewModel vm = new UsersViewModel()
            {
                GridControl = new GridControlModel()
                {
                    PageSize = 10,
                    CurrentPage = 1,
                    TotalRecords = 2
                },
                GridData = new List<UserGridRowModel>()
                {                    
                    new UserGridRowModel()
                    {
                        UserId = "1",
                        Email = "steve@intrepiddevops.com",
                        IsLocked = false,
                        IsTwoFactorEnabled = false,
                        Phone = "4165551234",
                        Username = "steve@intrepiddevops.com"
                    },
                    new UserGridRowModel()
                    {
                        UserId = "2",
                        Email = "john@intrepiddevops.com",
                        IsLocked = false,
                        IsTwoFactorEnabled = false,
                        Phone = "4165551234",
                        Username = "john@intrepiddevops.com"
                    }
                }
            };

            return View(vm);
        }

        public IActionResult AddUser(UserForCreationModel model)
        {
            ModelState.Clear();

            return View();
        }

        public IActionResult GetUser()
        {
            UserViewModel vm = new UserViewModel()
            {
                User = new UserForReadModel()
                {
                    UserId = "1",
                    Email = "john@intrepiddevops.com",
                    AccessFailedCount = 0,
                    IsEmailConfirmed = true,
                    IsPhoneConfirmed = true,
                    IsTwoFactorEnabled = false,
                    IsLockoutEnabled = true,
                    Username = "john@intrepiddevops.com",
                    Phone = "0000000000",
                    LockoutEnd = null,
                    Roles = new List<RoleForReadModel>()
                    {
                        new RoleForReadModel()
                        {
                            RoleId = "1",
                            Name = "Administrator",
                        },
                        new RoleForReadModel()
                        {
                            RoleId = "2",
                            Name = "Super User"
                        }
                    },
                    Claims = new List<ClaimForReadModel>()
                    {
                        new ClaimForReadModel()
                        {
                            Name = ClaimTypes.NameIdentifier,
                            Value = "1"
                        }
                    }
                }
            };

            return View(vm);
        }

        public IActionResult Roles()
        {
            RolesViewModel vm = new RolesViewModel()
            {
                GridControl = new GridControlModel()
                {
                    PageSize = 10,
                    CurrentPage = 1,
                    TotalRecords = 2
                },
                GridData = new List<RoleGridRowModel>()
                {
                    new RoleGridRowModel()
                    {
                        RoleId = "1",
                        Name = "Administrator"
                    },
                    new RoleGridRowModel()
                    {
                        RoleId = "2",
                        Name = "Super User"
                    }
                }
            };

            return View(vm);
        }
        
        public IActionResult Claims()
        {
            ClaimsViewModel vm = new ClaimsViewModel();

            foreach (FieldInfo field in typeof(ClaimTypes).GetFields())
            {
                vm.GridData.Add(new ClaimGridRowModel()
                {
                    ClaimId = null,
                    Name = field.Name,
                    Value = (string)field.GetValue(null),
                    IsReadOnly = true
                });
            }

            vm.GridControl = new GridControlModel()
            {
                PageSize = 10,
                CurrentPage = 1,
                TotalRecords = 2
            };

            return View(vm);
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}