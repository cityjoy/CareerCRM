using CareerCRM.App.Interface;
using CareerCRM.App.Request;
using CareerCRM.App.Response;
using CareerCRM.Mvc.Models.ViewModel;
using CareerCRM.Repository.Core;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace CareerCRM.Mvc.Controllers
{
    public class CodeGenerateController : BaseController
    {
        private readonly string projectName = "CareerCRM";//项目名称
        private readonly string controllerTxtPath = "";
        private readonly string controllerPath = "";
        private readonly string listViewModelTxtPath = "";
        private readonly string formDTOTxtPath = "";
        private readonly string applicationTxtPath = "";
        private readonly string listViewTxtPath = "";
        private readonly string searchViewModelTxtPath = "";
        private readonly string viewModelPath = "";
        private readonly string viewPath = "";
        private readonly string applacationPath = "";
        private readonly string controllerNs = "";
        private readonly string modelNS = "";
        private Dictionary<string, string> CTS_Dict = null;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public CodeGenerateController(IAuth authUtil, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider) : base(authUtil)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string loc = AppDomain.CurrentDomain.BaseDirectory;
            int indx = loc.IndexOf("bin");
            string path = loc.Substring(0, indx);
            controllerPath = path + "GeneratorFiles\\OutPut\\Controller\\";
            controllerTxtPath = path + "GeneratorFiles\\MvcTemplate\\Controller.txt";
            listViewModelTxtPath = path + "GeneratorFiles\\MvcTemplate\\ListViewModel.txt";
            formDTOTxtPath = path + "GeneratorFiles\\MvcTemplate\\FormDTO.txt";
            applicationTxtPath = path + "GeneratorFiles\\MvcTemplate\\AppLication.txt";
            searchViewModelTxtPath = path + "GeneratorFiles\\MvcTemplate\\ReqModel.txt";
            listViewTxtPath = path + "GeneratorFiles\\MvcTemplate\\ListView.txt";
            viewModelPath = path + "GeneratorFiles\\OutPut\\ViewModel\\";
            viewPath = path + "GeneratorFiles\\OutPut\\View\\";
            applacationPath = path + "GeneratorFiles\\OutPut\\Application\\";
            string controllerNs = typeof(BaseController).Namespace;
            string modelNS = projectName + "Repository.Domain";
            CTS_Dict = new Dictionary<string, string>(15)
            {
                { "SByte", "sbyte" },
                { "Int16", "short" },
                { "Int32", "int" },
                { "Int64", "long" },
                { "Byte", "byte" },
                { "UInt16", "ushort" },
                { "UInt32", "uint" },
                { "UInt64", "ulong" },
                { "Single", "float" },
                { "Double", "double" },
                { "Decimal", "decimal" },
                { "Boolean", "bool" },
                { "Char", "char" },
                { "Object", "object" },
                { "String", "string" },
                { "DateTime", "DateTime" }
            };
        }


        public IActionResult Index()
        {
            //var htmlContent = Render("~/Template/Temp.cshtml", new List<string> { "A","B","C"});
            return View();

        }


        /// <summary>
        /// 加载所有实体
        /// </summary>
        public JsonResult Load([FromQuery]PageReq request)
        {
            List<Type> list = GetAllTypes("*.Repository.dll");
            var result = list.Select(u => new { Name = u.Name })
                .Skip((request.page - 1) * request.limit)
                .Take(request.limit)
                .ToList();

            TableData model = new TableData
            {
                data = result,
                count = list.Count(),
            };
            return Json(model);
        }


        /// <summary>
        /// 加载实体所有属性
        /// </summary>
        public JsonResult LoadColumnList(string entityName)
        {
            List<Type> list = GetAllTypes("*.Repository.dll");
            Type result = list.Where(m => m.Name == entityName).First();
            var pros = result.GetProperties().Select(m => new { Name = m.Name,Desc = m.CustomAttributes.First().ConstructorArguments[0].Value, PropertyType = m.PropertyType.Name}).ToList();
             
            TableData model = new TableData
            {
                data = pros,
                count = pros.Count(),
            };
            return Json(model);
        }
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateCodes()
        {
            ICollection<string> formKeys = Request.ReadFormAsync().Result.Keys;
            List<string> list = new List<string>();
            foreach (string item in formKeys)
            {

                list.Add(item);

            }
            string entityName = Request.Form["entityName"];
            CreateCodeFiles(entityName, list);
            Result.Code = 200;
            Result.Message = "生成成功";
            return Json(Result);
        }
        /// <summary>
        /// 获取所有类型
        /// </summary>
        /// <param name="dllName"></param>
        /// <returns></returns>
        private List<Type> GetAllTypes(string dllName)
        {
            string path = Assembly.GetEntryAssembly().Location;
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(path));
            FileInfo dll = dir.GetFiles(dllName, SearchOption.AllDirectories).First();
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName);
            List<Type> result = assembly.GetExportedTypes().Where(x => typeof(Entity).IsAssignableFrom(x)).ToList();

            return result;
        }

        private void CreateCodeFiles(string modelName, List<string> fields)
        {

            GenerateController(modelName);//创建Controller文件
            GenerateViewModel(modelName, fields);//创建viewmodel文件
            GenerateApp(modelName, fields);//创建应用层文件
            GenerateView(modelName, fields);//创建View文件
            //System.IO.File.WriteAllText($"{viewModelPath}{modelName}VM.cs", GenerateVM(modelName,"CrudVM"), Encoding.UTF8);
            //System.IO.File.WriteAllText($"{viewModelPath}{modelName}ListVM.cs", GenerateVM("ListVM"), Encoding.UTF8);
            //System.IO.File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}Searcher.cs", GenerateVM("Searcher"), Encoding.UTF8);


        }



        /// <summary>
        /// 根据TXT模板路径读取文件
        /// </summary>
        /// <param name="txtPath"></param>
        /// <returns></returns>
        public string GetResourceContent(string txtPath)
        {
            StreamReader textStreamReader = new StreamReader(txtPath);
            string content = textStreamReader.ReadToEnd();
            textStreamReader.Close();
            return content;
        }
        /// <summary>
        /// 创建Controller文件
        /// </summary>
        /// <param name="modelName"></param>
        public void GenerateController(string modelName)
        {
            TempViewModel viewModel = new TempViewModel() { ProjectName = "CareerCRM", ControllerName = modelName, ModelName = modelName, DataList = null };
            string content = Render<TempViewModel>("~/Template/ControllerTemp.cshtml", viewModel);
            content = content.Replace("<p>", "").Replace("</p>", "");
            if (!Directory.Exists(controllerPath))
            {
                Directory.CreateDirectory(controllerPath);

            }
            System.IO.File.WriteAllText($"{controllerPath}{modelName}Controller.cs", content, Encoding.UTF8);
        }
        /// <summary>
        /// 创建ViewModel文件
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="fields"></param>
        public void GenerateViewModel(string modelName, List<string> fields)
        {
            List<string> searchList = fields.Where(m => m.StartsWith("search_")).ToList();//搜索字段
            List<string> fieldList = fields.Where(m => m.StartsWith("field_")).ToList();//列表字段
            List<string> formList = fields.Where(m => m.StartsWith("form_")).ToList();//表单字段

            List<Type> list = GetAllTypes("*.Repository.dll");
            Type modelType = list.Where(u => u.Name == modelName).FirstOrDefault();


            List<PropertyInfo> pros = modelType.GetProperties().ToList();

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>
            {
                { "searchList", searchList },
                { "fieldList", fieldList },
                { "formList", formList }
            };
            foreach (KeyValuePair<string, List<string>> item in dict)
            {
                string content = "";
                StringBuilder modelBuilder = new StringBuilder();
                StringBuilder viewBuilder = new StringBuilder();
                StringBuilder formBuilder = new StringBuilder();
                List<string> searchDataList = new List<string>();
                List<string> formDataList = new List<string>();
                List<string> tableDataList = new List<string>();
                foreach (string f in item.Value)
                {
                    string fieldName = "";
                    if (item.Key == "searchList")
                    {
                        fieldName = f.Replace("search_", "").Trim();
                    }
                    if (item.Key == "fieldList")
                    {
                        fieldName = f.Replace("field_", "").Trim();
                    }
                    if (item.Key == "formList")
                    {
                        fieldName = f.Replace("form_", "").Trim();
                    }
                    PropertyInfo pro = pros.Where(m => m.Name == fieldName).FirstOrDefault();
                    Type t = pro.PropertyType;
                    string fieldType = "string";
                    string descriptionName = "";
                    DescriptionAttribute[] v = (DescriptionAttribute[])pro.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (v.Count() > 0)
                    {
                        string description = v[0].Description;
                        descriptionName = description.Length > 0 ? description : fieldName;

                    }
                    if (CTS_Dict.ContainsKey(t.Name))
                    {

                        fieldType = CTS_Dict[t.Name];
                    }
                    //modelBuilder.Append("\r\n");
                    //modelBuilder.Append("/// <summary>\r\n");
                    //modelBuilder.Append($"///{descriptionName}\r\n");
                    //modelBuilder.Append("/// </summary>\r\n");
                    //modelBuilder.Append($"[Description(\"{descriptionName}\")] \r\n");
                    //modelBuilder.Append("public " + fieldType + " " + fieldName + " { get; set; } \r\n");
                    if (item.Key == "fieldList")
                    {
                        tableDataList.Add(fieldName + "," + descriptionName + "," + fieldType);
                    }
                    else if (item.Key == "formList")
                    {
                        formDataList.Add(fieldName + "," + descriptionName + "," + fieldType);
                    }
                    else if (item.Key == "searchList")
                    {
                        searchDataList.Add(fieldName + "," + descriptionName + "," + fieldType);
                    }

                }

                if (!Directory.Exists(viewModelPath))
                {
                    Directory.CreateDirectory(viewModelPath);

                }

                if (searchDataList.Count > 0)
                {
                    TempViewModel viewModel = new TempViewModel() { ProjectName = "CareerCRM", ControllerName = modelName, ModelName = modelName, DataList = searchDataList };
                    content = Render<TempViewModel>("~/Template/ReqModelTemp.cshtml", viewModel);
                    content = content.Replace("<p>", "").Replace("</p>", "");

                    System.IO.File.WriteAllText($"{viewModelPath}{modelName}ListReq.cs", content, Encoding.UTF8);
                }

                if (tableDataList.Count > 0)
                {
                    TempViewModel viewModel = new TempViewModel() { ProjectName = "CareerCRM", ControllerName = modelName, ModelName = modelName, DataList = tableDataList };
                    content = Render<TempViewModel>("~/Template/ListViewModelTemp.cshtml", viewModel);
                    content = content.Replace("<p>", "").Replace("</p>", "");

                    System.IO.File.WriteAllText($"{viewModelPath}{modelName}ListVM.cs", content, Encoding.UTF8);
                }

                if (formDataList.Count > 0)
                {
                    TempViewModel viewModel = new TempViewModel() { ProjectName = "CareerCRM", ControllerName = modelName, ModelName = modelName, DataList = formDataList };
                    content = Render<TempViewModel>("~/Template/FormDTOTemp.cshtml", viewModel);
                    content = content.Replace("<p>", "").Replace("</p>", "");

                    System.IO.File.WriteAllText($"{viewModelPath}{modelName}FormDTO.cs", content, Encoding.UTF8);
                }


            }


        }
        /// <summary>
        /// 生成应用层文件
        /// </summary>
        /// <param name="modelName"></param>
        public void GenerateApp(string modelName, List<string> fields)
        {
            List<string> formList = fields.Where(m => m.StartsWith("form_")).ToList();//表单字段
            List<string> listViewList = fields.Where(m => m.StartsWith("field_")).ToList();//表单字段
            List<string> searchList = fields.Where(m => m.StartsWith("search_")).ToList();//表单字段
            List<Type> list = GetAllTypes("*.Repository.dll");
            Type modelType = list.Where(u => u.Name == modelName).FirstOrDefault();
            List<PropertyInfo> pros = modelType.GetProperties().ToList();
            List<string> searchDataList = new List<string>();
                foreach(string f in searchList)
            {
                string fieldName = f.Replace("search_", "").Trim();
                PropertyInfo pro = pros.Where(m => m.Name == fieldName).FirstOrDefault();
                Type t = pro.PropertyType;
                string fieldType = "string";
                string descriptionName = "";
                DescriptionAttribute[] v = (DescriptionAttribute[])pro.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (v.Count() > 0)
                {
                    string description = v[0].Description;
                    descriptionName = description.Length > 0 ? description : fieldName;

                }
                if (CTS_Dict.ContainsKey(t.Name))
                {

                    fieldType = CTS_Dict[t.Name];
                }
                searchDataList.Add(fieldName + "," + descriptionName + "," + fieldType);

            }
            TempViewModel viewModel = new TempViewModel() { ProjectName = "CareerCRM", ControllerName = modelName, ModelName = modelName, DataList = formList, ListViewDataList = listViewList, SearchDataList = searchDataList };
            string content = Render<TempViewModel>("~/Template/AppLicationTemp.cshtml", viewModel);
            content = content.Replace("<p>", "").Replace("</p>", "");
            System.IO.File.WriteAllText($"{applacationPath}{modelName}App.cs", content, Encoding.UTF8);


        }

        /// <summary>
        /// 创建View文件
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="fields"></param>
        public void GenerateView(string modelName, List<string> fields)
        {
            List<string> searchList = fields.Where(m => m.StartsWith("search_")).ToList();//搜索字段
            List<string> fieldList = fields.Where(m => m.StartsWith("field_")).ToList();//列表字段
            List<string> formList = fields.Where(m => m.StartsWith("form_")).ToList();//表单字段
            List<string> contentList = fields.Where(m => m.StartsWith("content_")).ToList();//需要设置的富文本字段

            List<Type> list = GetAllTypes("*.Repository.dll");
            Type modelType = list.Where(u => u.Name == modelName).FirstOrDefault();


            List<PropertyInfo> pros = modelType.GetProperties().ToList();

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>
            {
                { "searchList", searchList },
                { "fieldList", fieldList },
                { "formList", formList },
                { "contentList", contentList }
                
            };

            List<string> searchDataList = new List<string>();
            List<string> formDataList = new List<string>();
            List<string> tableDataList = new List<string>();
            List<string> contentDataList = new List<string>();

            foreach (KeyValuePair<string, List<string>> item in dict)
            {

                foreach (string f in item.Value)
                {
                    string fieldName = "";
                    if (item.Key == "searchList")
                    {
                        fieldName = f.Replace("search_", "").Trim();
                    }
                    else if (item.Key == "fieldList")
                    {
                        fieldName = f.Replace("field_", "").Trim();
                    }
                    else if (item.Key == "formList")
                    {
                        fieldName = f.Replace("form_", "").Trim();
                    }
                    else if (item.Key == "contentList")
                    {
                        fieldName = f.Replace("content_", "").Trim();
                    }
                    PropertyInfo pro = pros.Where(m => m.Name == fieldName).FirstOrDefault();
                    Type t = pro.PropertyType;
                    string fieldType = "string";
                    string descriptionName = "";
                    DescriptionAttribute[] v = (DescriptionAttribute[])pro.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (v.Count() > 0)
                    {
                        string description = v[0].Description;
                        descriptionName = description.Length > 0 ? description : fieldName;

                    }
                    if (CTS_Dict.ContainsKey(t.Name))
                    {

                        fieldType = CTS_Dict[t.Name];
                    }
                    if (item.Key == "searchList")
                    {
                        searchDataList.Add(fieldName + "," + descriptionName + "," + fieldType);
                    }
                    else if (item.Key == "fieldList")
                    {
                        tableDataList.Add(fieldName + "," + descriptionName + "," + fieldType);
                    }
                    else if (item.Key == "formList")
                    {
                        formDataList.Add(fieldName + "," + descriptionName + "," + fieldType);
                    }
                    else if (item.Key == "contentList")
                    {
                        contentDataList.Add(fieldName);
                    }


                }

            }
            string content = "";
            if (!Directory.Exists(viewPath))
            {
                Directory.CreateDirectory(viewPath);

            }
            TempViewModel viewModel = new TempViewModel()
            {
                ProjectName = "CareerCRM",
                ControllerName = modelName,
                ModelName = modelName,
                DataList = formDataList,
                SearchDataList = searchDataList,
                ContentDataList = contentDataList,
                ListViewDataList = tableDataList
            };
            if (tableDataList.Count > 0)//生成列表视图
            {
                
                content = Render<TempViewModel>("~/Template/ListViewTemp.cshtml", viewModel);

                System.IO.File.WriteAllText($"{viewPath}{modelName}List.cshtml", content, Encoding.UTF8);
            }

            //string content = GetResourceContent(listViewTxtPath).Replace("$column$", viewBuilder.ToString()).Replace("$modelname$", modelName);
            //System.IO.File.WriteAllText($"{viewPath}{modelName}List.cshtml", content, Encoding.UTF8);

            //string formText = formBuilder.ToString();
            //content = GetResourceContent(listViewTxtPath).Replace("$column$", viewBuilder.ToString()).Replace("$modelname$", modelName).Replace("$formFields$", formText);
            //System.IO.File.WriteAllText($"{viewPath}Edit.cshtml", content, Encoding.UTF8);
            if (formDataList.Count > 0)//生成表单,详情视图
            {
                content = Render<TempViewModel>("~/Template/EditViewTemp.cshtml", viewModel);
                System.IO.File.WriteAllText($"{viewPath}Edit.cshtml", content, Encoding.UTF8);
                content = Render<TempViewModel>("~/Template/DetailViewTemp.cshtml", viewModel);
                System.IO.File.WriteAllText($"{viewPath}Detail.cshtml", content, Encoding.UTF8);

            }


        }
        /// <summary>
        /// 使用Razor视图引擎来生成代码文件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string Render<TModel>(string viewPath, TModel model)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            ActionContext actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult viewResult = _viewEngine.GetView(null, viewPath, false);
            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"找不到视图模板 {viewPath}");
            }

            ViewDataDictionary viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            using (StringWriter writer = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(
                     actionContext,
                     viewResult.View,
                     viewDictionary,
                     new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                     writer,
                     new HtmlHelperOptions()
                 );
                Task render = viewResult.View.RenderAsync(viewContext);
                render.Wait();
                return writer.ToString();
            }
        }
    }
}