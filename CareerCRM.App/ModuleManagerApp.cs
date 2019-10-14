using System.Collections.Generic;
using System.Linq;
using CareerCRM.Repository.Domain;
using CareerCRM.Repository.Interface;

namespace CareerCRM.App
{
    public class ModuleManagerApp :BaseApp<Module>
    {
        private RevelanceManagerApp _revelanceApp;
        IRepository<ModuleElement> _moduleElementRepository;


        public ModuleManagerApp(IUnitWork unitWork,
        IRepository<ModuleElement> moduleElementRepository,
        IRepository<Module> repository
, RevelanceManagerApp app) : base(unitWork, repository)
        {
            _revelanceApp = app;
            _moduleElementRepository = moduleElementRepository;
        }
        public void Add(Module model)
        {
            ChangeModuleCascade(model);
            Repository.Add(model);
        }

        public void Update(Module model)
        {
            ChangeModuleCascade(model);
            Repository.Update(model);
        }

        #region 用户/角色分配模块


        /// <summary>
        /// 加载特定角色的模块
        /// </summary>
        /// <param name="roleId">The role unique identifier.</param>
        public IEnumerable<Module> LoadForRole(string roleId)
        {
            var moduleIds = UnitWork.Find<Relevance>(u => u.FirstId == roleId && u.Key == Define.ROLEMODULE)
                .Select(u => u.SecondId);
            return UnitWork.Find<Module>(u => moduleIds.Contains(u.Id) && u.IsDeleted == false).OrderBy(u => u.SortNo);
        }

        //获取角色可访问的模块字段
        public IEnumerable<string> LoadPropertiesForRole(string roleId, string moduleCode)
        {
            return _revelanceApp.Get(Define.ROLEDATAPROPERTY, roleId, moduleCode);
        }

        public IEnumerable<ModuleElement> LoadMenusForRole(string moduleId, string roleId)
        {
            var elementIds = _revelanceApp.Get(Define.ROLEELEMENT, true, roleId);
            var query = UnitWork.Find<ModuleElement>(u => elementIds.Contains(u.Id));
            if (!string.IsNullOrEmpty(moduleId))
            {
               query =  query.Where(u => u.ModuleId == moduleId);
            }

            return query;
        }

        #endregion 用户/角色分配模块


        #region 菜单操作
        /// <summary>
        /// 删除指定的菜单
        /// </summary>
        /// <param name="ids"></param>
        public void DelMenu(string[] ids)
        {
            _moduleElementRepository.Delete(u => ids.Contains(u.Id));
            UnitWork.Save();
        }

        public void AddMenu(ModuleElement model)
        {
            _moduleElementRepository.Add(model);
            UnitWork.Save();
        }
        #endregion

        public void UpdateMenu(ModuleElement model)
        {
            _moduleElementRepository.Update(model);
            UnitWork.Save();
        }


        /// <summary>
        /// 软删除指定模块
        /// </summary>
        /// <param name="ids"></param>
        public void DelModule(string[] ids)
        {
            Repository.Update(u => ids.Contains(u.Id), u => new Module{ IsDeleted = true });//u =>u.Id==1,u =>new User{Name="ok"}
            //UnitWork.Save();
        }
    }
}