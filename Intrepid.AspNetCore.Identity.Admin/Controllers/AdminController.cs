using System;
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
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Intrepid.AspNetCore.Identity.Admin.Configuration;

namespace Intrepid.AspNetCore.Identity.Admin.Controllers
{
    [Route("admin")]
    [Authorize(Policy = "AdminManagerRole")]
    public class AdminController : BaseController
    {
        public RoleService RoleService { get; }
        public IMapper Mapper { get; }
        public IdentityService IdentityService { get; }

        private readonly IdentityDataConfiguration _identityConfiguration;

        public AdminController(RoleService roleService, IMapper mapepr, 
            IdentityService identityService, IdentityDataConfiguration identityConfiguration)
        {
            this.RoleService = roleService;
            this.Mapper = mapepr;
            this.IdentityService = identityService;
            this._identityConfiguration = identityConfiguration;
        }
        /// <summary>
        /// Dashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
            };

            return View(vm);
        }
        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var results = await IdentityService.AllUsers(1000, 0);
            var users = results.Item2.Select(x => Mapper.Map<UserGridRowModel>(x)).ToList();
            UsersViewModel vm = new UsersViewModel()
            {
                GridControl = new GridControlModel()
                {
                    PageSize = 10,
                    CurrentPage = 1,
                    TotalRecords = results.count
                },
                GridData = users
            };

            return View(vm);
        }
        [HttpGet("user/{id}")]
        public async Task<IActionResult> UserDetail(string id)
        {
            var user=await this.IdentityService.SearchUserByIdAsync(id);
            
            if (user == default)
                return RedirectToAction("users");
            var model = this.Mapper.Map<UserForCreationModel>(user);
            model.AvailableRoles= (await this.RoleService.AllRoleInfo()).ReturnObject.ToDictionary(x => x.Id, x => x.Name);
            return View("UserDetail", model);
            
            
        }
        [HttpGet("adduser")]
        public async Task<IActionResult> AddUser()
        {
            var model = new UserForCreationModel();
            var roles = await this.RoleService.AllRoleInfo();
            model.AvailableRoles = roles.ReturnObject.ToDictionary(x => x.Id, x => x.Name);
            ModelState.Clear();

            return View("userdetail", model);
        }
        [HttpPost("addorupdate")]
        public async Task<IActionResult> AddOrUpdate(UserForCreationModel vm)
        {
            var roles = await this.RoleService.AllRoleInfo();
            vm.AvailableRoles = roles.ReturnObject.ToDictionary(x => x.Id, x => x.Name);
            if (!ModelState.IsValid)
                return View("UserDetail", vm);
            vm.Username = vm.Email;
            
            var identityUser = this.Mapper.Map<IdentityUserDTO>(vm);
            if (string.IsNullOrEmpty(identityUser.Id))
            {
                identityUser.Id = Guid.NewGuid().ToString();
                var identityUserCreateResule = await this.IdentityService.CreateUser(identityUser, vm.Password);
                if (!identityUserCreateResule.IsSuccess)
                {
                    this.ModelState.AddModelError(nameof(vm.Username), string.Join(", ", identityUserCreateResule.IdentityError.Select(x => x.Description)));
                    return View("UserDetail", vm);
                }
            }
            else
            {
                identityUser.PasswordHash = string.Empty;
                var updateUserCreateResule = await this.IdentityService.UpdateUser(identityUser, vm.Password);
                if (!updateUserCreateResule.IsSuccess)
                {
                    this.ModelState.AddModelError(nameof(vm.Username), string.Join(", ", updateUserCreateResule.IdentityError.Select(x => x.Description)));
                    return View("UserDetail", vm);
                }
            }
            return RedirectToAction("Users");
        }

        [HttpPost("deleteuser")]
        public async Task<IActionResult> DeleteUser(UserForCreationModel vm)
        {
            var roles = await this.RoleService.AllRoleInfo();
            vm.AvailableRoles = roles.ReturnObject.ToDictionary(x => x.Id, x => x.Name);
            
            //cannot delete ur self
            if (vm.UserId.ToLower() == User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower())
            {
                this.ModelState.AddModelError(nameof(vm.Email), "cannot delete the current logged in user");
                return View("UserDetail", vm);
            }
            var result = await this.IdentityService.DeleteUserAsync(vm.UserId);
            if (!result.IsSuccess)
            {
                this.ModelState.AddModelError(nameof(vm.Email), string.Join(", ", result.ErrorMsg));
                return View("UserDetail", vm);
            }
            return RedirectToAction("users");
        }
        [HttpGet("getuser")]
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
        [HttpGet("Roles")]
        public async Task<IActionResult> Roles()
        {
            var roles = await this.RoleService.AllRoleInfo();
            //project the roles
            var rolesModel = roles.ReturnObject.Select(x => this.Mapper.Map<RoleCountModel>(x)).ToList();

            RolesViewModel vm = new RolesViewModel()
            {
                GridControl = new GridControlModel()
                {
                    PageSize = 10,
                    CurrentPage = 1,
                    TotalRecords = rolesModel.Count,
                },
                GridData = new List<RoleGridRowModel>()
            };
            foreach(var role in rolesModel)
            {
                vm.GridData.Add(new RoleGridRowModel()
                {
                    RoleId = role.RoleId,
                    Name = role.Name,
                    Count = role.Count
                });
            }

            return View(vm);
        }

        [HttpGet("newrole")]
        public IActionResult AddRoleForm(RoleCountModel newRole)
        {
            var vm = new RoleCountModel();
            vm.AvailableClaims = this._identityConfiguration.Claims;
            return View("RoleDetail", vm);
        }
        [HttpGet("role/{id}")]
        public async  Task<IActionResult> AddRoleForm(string id)
        {
            var role = (await this.RoleService.GetRole(id)).ReturnObject.Where(x => x.Id == id).FirstOrDefault();
            if (role == default)
                return RedirectToAction("Roles");
            var vm = new RoleCountModel()
            {
                RoleId = role.Id,
                Name = role.Name,
                Count = role.UserCount,
                ConcurrencyStamp = role.ConcurrencyStamp,
                SelectedClaims=role.Claims,
                AvailableClaims = this._identityConfiguration.Claims
            };
            return View("RoleDetail", vm);
        }
        [HttpPost("addupdaterole")]
        public async Task<IActionResult> AddUpdateRole(RoleCountModel newRole)
        {
            newRole.AvailableClaims = this._identityConfiguration.Claims;
            if (string.IsNullOrEmpty(newRole.Name))
                return RedirectToAction("Roles");
            //check if the role existed already
            
            var result=await this.RoleService.CreateUpdateRole(this.Mapper.Map< IdentityRoleDTO>(newRole));
            if (result.IsSuccess)
                return RedirectToAction("Roles");
            //ok it failed
            //ok it has failed// publish to the model state
            this.ModelState.AddModelError(nameof(newRole.Name), string.Join(", ", result.ErrorMsg));
            this.ModelState.AddModelError(nameof(newRole.Name), string.Join(", ", result.IdentityError.Select(x=>x.Description)));
            return View("RoleDetail", newRole);
        }
        [HttpPost("deleterole")]
        public async Task<IActionResult> Delete(RoleCountModel vm)
        {
            if (string.IsNullOrEmpty(vm.RoleId))
                return RedirectToAction("Roles");
            //check if the role existed already

            var result = await this.RoleService.DeleteRole(vm.RoleId);
            if (result.IsSuccess)
                return RedirectToAction("Roles");
            //ok it failed
            //ok it has failed// publish to the model state
            this.ModelState.AddModelError(nameof(vm.Name), string.Join(", ", result.ErrorMsg));
            this.ModelState.AddModelError(nameof(vm.Name), string.Join(", ", result.IdentityError.Select(x => x.Description)));
            return View("RoleDetail");
        }
        [HttpGet("claims")]
        public IActionResult Claims()
        {
            ClaimsViewModel vm = new ClaimsViewModel();

            foreach (var claim in this.User.Claims)
            {
                vm.GridData.Add(new ClaimGridRowModel()
                {
                    ClaimId = null,
                    Name = claim.Type,
                    Value = claim.Value,
                    IsReadOnly = true
                });
            }

            vm.GridControl = new GridControlModel()
            {
                PageSize = 10,
                CurrentPage = 1,
                TotalRecords = this.User.Claims.Count()
            };

            return View(vm);
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}