using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using CareerCRM.App.Response;
using CareerCRM.Repository.Domain;
using CareerCRM.Repository.Interface;

namespace CareerCRM.App
{
    /// <summary>
    /// 普通用户授权策略
    /// </summary>
    public class NormalAuthStrategy :BaseApp<User>, IAuthStrategy
    {
        
        protected User _user;

        private List<string> _userRoleIds;    //用户角色GUID
        private DbExtension _dbExtension;

        public List<ModuleView> Modules
        {
            get {//如果是超级管理员加载所有模块
                if (_user.IsMaster==1)
                {
                    
                        var modules = (from module in UnitWork.Find<Module>(u=>u.IsDeleted==false)
                                       select new ModuleView
                                       {
                                           Name = module.Name,
                                           Id = module.Id,
                                           CascadeId = module.CascadeId,
                                           Code = module.Code,
                                           IconName = module.IconName,
                                           Url = module.Url,
                                           ParentId = module.ParentId,
                                           ParentName = module.ParentName,
                                           IsSys = module.IsSys
                                       }).ToList();

                        foreach (var module in modules)
                        {
                            module.Elements = UnitWork.Find<ModuleElement>(u => u.ModuleId == module.Id).ToList();
                        }

                        return modules;
                   
                }
                else
                {
                    var moduleIds = UnitWork.Find<Relevance>(
                        u =>
                            (u.Key == Define.ROLEMODULE && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);

                    var modules = (from module in UnitWork.Find<Module>(u => moduleIds.Contains(u.Id)&& u.IsDeleted == false)
                                   select new ModuleView
                                   {
                                       Name = module.Name,
                                       Code = module.Code,
                                       CascadeId = module.CascadeId,
                                       Id = module.Id,
                                       IconName = module.IconName,
                                       Url = module.Url,
                                       ParentId = module.ParentId,
                                       ParentName = module.ParentName,
                                       IsSys = module.IsSys
                                   }).ToList();

                    var usermoduleelements = ModuleElements;

                    foreach (var module in modules)
                    {
                        module.Elements = usermoduleelements.Where(u => u.ModuleId == module.Id).ToList();
                    }
                    return modules;

                }
            }
        }

        public List<ModuleElement> ModuleElements
        {
            get
            {
                //如果是超级管理员加载所有模块
                if (_user.IsMaster==1)
                {
                    return UnitWork.Find<ModuleElement>(null).ToList();
                }
                else
                {
                    var elementIds = UnitWork.Find<Relevance>(
                    u =>
                        (u.Key == Define.ROLEELEMENT && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);
                    var usermoduleelements = UnitWork.Find<ModuleElement>(u => elementIds.Contains(u.Id));
                    return usermoduleelements.ToList();
                }
            }
        }

        public List<Role> Roles
        {
            get
            {
                //如果是超级管理员加载所有模块
                if (_user.IsMaster==1)
                {
                    return UnitWork.Find<Role>(null).ToList();
                }
                else
                {
                    return UnitWork.Find<Role>(u => _userRoleIds.Contains(u.Id)).ToList();
                }
            }
        }

        public List<Resource> Resources
        {
            get
            {
                //如果是超级管理员加载所有模块
                if (_user.IsMaster==1)
                {
                    return UnitWork.Find<Resource>(null).ToList();
                }
                else
                {
                    var resourceIds = UnitWork.Find<Relevance>(
                        u =>
                            (u.Key == Define.ROLERESOURCE && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);
                    return UnitWork.Find<Resource>(u => resourceIds.Contains(u.Id)).ToList();
                }
            }
        }

        public List<Org> Orgs
        {
            get
            {
                //如果是超级管理员加载所有模块
                if (_user.IsMaster==1)
                {
                    return UnitWork.Find<Org>(null).ToList();
                }
                else
                {
                    var orgids = UnitWork.Find<Relevance>(
                    u =>
                        (u.FirstId == _user.Id && u.Key == Define.USERORG) ||
                        (u.Key == Define.ROLEORG && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);
                    return UnitWork.Find<Org>(u => orgids.Contains(u.Id)).ToList();
                }
            }
        }

        public User User
        {
            get { return _user; }
            set
            {
                    _user = value;
                    _userRoleIds = UnitWork.Find<Relevance>(u => u.FirstId == _user.Id && u.Key == Define.USERROLE).Select(u => u.SecondId).ToList();
            }
        }

        /// <summary>
        /// 获取用户可访问的字段列表
        /// </summary>
        /// <param name="moduleCode">模块的code</param>
        /// <returns></returns>
        public List<KeyDescription> GetProperties(string moduleCode)
        {
            var allprops = _dbExtension.GetProperties(moduleCode);
            if (_user.IsMaster==1)
            {
                return allprops;
            }
            var props =UnitWork.Find<Relevance>(u =>
                    u.Key == Define.ROLEDATAPROPERTY && _userRoleIds.Contains(u.FirstId) && u.SecondId == moduleCode)
                .Select(u => u.ThirdId);

            return allprops.Where(u => props.Contains(u.Key)).ToList();
        }

        public NormalAuthStrategy(IUnitWork unitWork, IRepository<User> repository, DbExtension dbExtension) : base(unitWork, repository)
        {
            _dbExtension = dbExtension;
        }
    }
}