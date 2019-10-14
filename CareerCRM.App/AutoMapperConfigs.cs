using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using CareerCRM.Repository.Domain;
using CareerCRM.App.Response;

namespace CareerCRM.App
{
    public class AutoMapperProfile : Profile
    {
        //添加你的实体映射关系.
        public AutoMapperProfile()
        {
            //GoodsEntity转GoodsDto.
            CreateMap<News, NewsListVM>();
                //映射发生之前
                //.BeforeMap((source, dto) => {
                //    //可以较为精确的控制输出数据格式
                //    dto.CreateTime = Convert.ToDateTime(source.CreateTime).ToString("yyyy-MM-dd");
                //})
                //映射发生之后
                //.AfterMap((source, dto) => {
                //    //code ...
                //});
        }
    }
}
