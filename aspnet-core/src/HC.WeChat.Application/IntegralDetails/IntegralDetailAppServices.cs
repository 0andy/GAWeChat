﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;

using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using HC.WeChat.IntegralDetails.Authorization;
using HC.WeChat.IntegralDetails.Dtos;
using HC.WeChat.IntegralDetails.DomainServices;
using HC.WeChat.IntegralDetails;
using System;
using HC.WeChat.Authorization;
using HC.WeChat.WeChatUsers;
using System.Linq;
using HC.WeChat.WechatEnums;
using HC.WeChat.WeChatUsers.Dtos;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs;
using HC.WeChat.WechatAppConfigs.Dtos;
using HC.WeChat.WechatAppConfigs;

namespace HC.WeChat.IntegralDetails
{
    /// <summary>
    /// IntegralDetail应用层服务的接口实现方法
    /// </summary>
    //[AbpAuthorize(IntegralDetailAppPermissions.IntegralDetail)]
    [AbpAuthorize(AppPermissions.Pages)]
    public class IntegralDetailAppService : WeChatAppServiceBase, IIntegralDetailAppService
    {
        ////BCC/ BEGIN CUSTOM CODE SECTION
        ////ECC/ END CUSTOM CODE SECTION
        private readonly IRepository<IntegralDetail, Guid> _integraldetailRepository;
        private readonly IIntegralDetailManager _integraldetailManager;
        private readonly IRepository<WeChatUser, Guid> _wechatusersRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        public IntegralDetailAppService(IRepository<IntegralDetail, Guid> integraldetailRepository
      , IIntegralDetailManager integraldetailManager, IRepository<WeChatUser, Guid> wechatusersRepository)
        {
            _integraldetailRepository = integraldetailRepository;
            _integraldetailManager = integraldetailManager;
            _wechatusersRepository = wechatusersRepository;
        }

        /// <summary>
        /// 获取IntegralDetail的分页列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //public async Task<PagedResultDto<IntegralDetailListDto>> GetPagedIntegralDetails(GetIntegralDetailsInput input)
        //{

        //    var query = _integraldetailRepository.GetAll();
        //    //TODO:根据传入的参数添加过滤条件
        //    var integraldetailCount = await query.CountAsync();

        //    var integraldetails = await query
        //        .OrderBy(input.Sorting)
        //        .PageBy(input)
        //        .ToListAsync();

        //    //var integraldetailListDtos = ObjectMapper.Map<List <IntegralDetailListDto>>(integraldetails);
        //    var integraldetailListDtos = integraldetails.MapTo<List<IntegralDetailListDto>>();

        //    return new PagedResultDto<IntegralDetailListDto>(
        //        integraldetailCount,
        //        integraldetailListDtos
        //        );

        //}

        /// <summary>
        /// 通过指定id获取IntegralDetailListDto信息
        /// </summary>
        public async Task<IntegralDetailListDto> GetIntegralDetailByIdAsync(EntityDto<Guid> input)
        {
            var entity = await _integraldetailRepository.GetAsync(input.Id);

            return entity.MapTo<IntegralDetailListDto>();
        }

        /// <summary>
        /// 导出IntegralDetail为excel表
        /// </summary>
        /// <returns></returns>
        //public async Task<FileDto> GetIntegralDetailsToExcel(){
        //var users = await UserManager.Users.ToListAsync();
        //var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
        //await FillRoleNames(userListDtos);
        //return _userListExcelExporter.ExportToFile(userListDtos);
        //}
        /// <summary>
        /// MPA版本才会用到的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<GetIntegralDetailForEditOutput> GetIntegralDetailForEdit(NullableIdDto<Guid> input)
        {
            var output = new GetIntegralDetailForEditOutput();
            IntegralDetailEditDto integraldetailEditDto;

            if (input.Id.HasValue)
            {
                var entity = await _integraldetailRepository.GetAsync(input.Id.Value);

                integraldetailEditDto = entity.MapTo<IntegralDetailEditDto>();

                //integraldetailEditDto = ObjectMapper.Map<List <integraldetailEditDto>>(entity);
            }
            else
            {
                integraldetailEditDto = new IntegralDetailEditDto();
            }

            output.IntegralDetail = integraldetailEditDto;
            return output;

        }

        /// <summary>
        /// 添加或者修改IntegralDetail的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateIntegralDetail(CreateOrUpdateIntegralDetailInput input)
        {

            if (input.IntegralDetail.Id.HasValue)
            {
                await UpdateIntegralDetailAsync(input.IntegralDetail);
            }
            else
            {
                await CreateIntegralDetailAsync(input.IntegralDetail);
            }
        }

        /// <summary>
        /// 新增IntegralDetail
        /// </summary>
        //[AbpAuthorize(IntegralDetailAppPermissions.IntegralDetail_CreateIntegralDetail)]
        protected virtual async Task<IntegralDetailEditDto> CreateIntegralDetailAsync(IntegralDetailEditDto input)
        {
            //TODO:新增前的逻辑判断，是否允许新增
            var entity = ObjectMapper.Map<IntegralDetail>(input);

            entity = await _integraldetailRepository.InsertAsync(entity);
            return entity.MapTo<IntegralDetailEditDto>();
        }

        /// <summary>
        /// 编辑IntegralDetail
        /// </summary>
        //[AbpAuthorize(IntegralDetailAppPermissions.IntegralDetail_EditIntegralDetail)]
        protected virtual async Task UpdateIntegralDetailAsync(IntegralDetailEditDto input)
        {
            //TODO:更新前的逻辑判断，是否允许更新
            var entity = await _integraldetailRepository.GetAsync(input.Id.Value);
            input.MapTo(entity);

            // ObjectMapper.Map(input, entity);
            await _integraldetailRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除IntegralDetail信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(IntegralDetailAppPermissions.IntegralDetail_DeleteIntegralDetail)]
        public async Task DeleteIntegralDetail(EntityDto<Guid> input)
        {

            //TODO:删除前的逻辑判断，是否允许删除
            await _integraldetailRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 批量删除IntegralDetail的方法
        /// </summary>
        //[AbpAuthorize(IntegralDetailAppPermissions.IntegralDetail_BatchDeleteIntegralDetails)]
        public async Task BatchDeleteIntegralDetailsAsync(List<Guid> input)
        {
            //TODO:批量删除前的逻辑判断，是否允许删除
            await _integraldetailRepository.DeleteAsync(s => input.Contains(s.Id));
        }

        /// <summary>
        /// 获取IntegralDetail的分页列表用户积分汇总信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<IntegralDetailListDto>> GetPagedIntegralDetailsAsync(GetIntegralDetailsInput input)
        {
            var queryIntegralDetail = _integraldetailRepository.GetAll();
            var queryWXUser = _wechatusersRepository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.Filter), v => v.NickName.Contains(input.Filter))
                .WhereIf(input.UserType.HasValue, u => u.UserType == input.UserType)
                .WhereIf(!string.IsNullOrEmpty(input.Phone), u => u.Phone.Contains(input.Phone));

            var queryGroup = from i in queryIntegralDetail
                             group new { i.OpenId, i.Integral } by new
                             {
                                 i.OpenId,
                             } into r
                             select new IntegralDetailListDto()
                             {
                                 FinalIntegral = r.Sum(v => v.Integral),
                                 OpenId = r.Key.OpenId,
                             };
            var query = from g in queryGroup
                        join u in queryWXUser on g.OpenId equals u.OpenId
                        select new IntegralDetailListDto()
                        {
                            OpenId = u.OpenId,
                            FinalIntegral = g.FinalIntegral,
                            WXName = u.NickName,
                            Phone = u.Phone,
                            UserTypeName = Enum.GetName(typeof(UserTypeEnum), u.UserType),
                        };

            ////TODO:根据传入的参数添加过滤条件
            var intergralCount = await query.CountAsync();

            var intergral = await query
                .OrderByDescending(s => s.CreationTime)
                .ThenBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var intergralListDtos = intergral.MapTo<List<IntegralDetailListDto>>();

            return new PagedResultDto<IntegralDetailListDto>(
                            intergralCount,
                            intergralListDtos
                            );
        }

        /// <summary>
        /// 根据OpenId查询用户积分详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<IntegralDetailListDto>> GetPagedIntegralDetailsByIdAsync(GetIntegralDetailsInput input)
        {
            var queryIntegralDetail = _integraldetailRepository.GetAll().Where(v=>v.OpenId == input.OpenId);
            var queryWXUser = _wechatusersRepository.GetAll();
            var query = from i in queryIntegralDetail
                        join u in queryWXUser on i.OpenId equals u.OpenId
                        select new IntegralDetailListDto()
                        {
                            OpenId = i.OpenId,
                            Integral = i.Integral,
                            InitialIntegral = i.InitialIntegral,
                            FinalIntegral = i.FinalIntegral,
                            //TypeName = Enum.GetName(typeof(IntegralTypeEnum),i.Type),
                            Type = i.Type,
                            WXName = u.NickName,
                            CreationTime = i.CreationTime,
                        };

            ////TODO:根据传入的参数添加过滤条件
            var intergralCount = await query.CountAsync();

            var intergral = await query
                .OrderByDescending(s => s.CreationTime)
                .ThenBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var intergralListDtos = intergral.MapTo<List<IntegralDetailListDto>>();

            return new PagedResultDto<IntegralDetailListDto>(
                            intergralCount,
                            intergralListDtos
                            );
        }

        /// <summary>
        /// 查询用户基本信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<WeChatUserListDto> GetUserInfoAsync(string openId)
        {
            var entity = await (from u in _wechatusersRepository.GetAll().Where(v => v.OpenId == openId)
                         select new WeChatUserListDto()
                         {
                             OpenId = u.OpenId,
                             Phone = u.Phone,
                             MemberBarCode = u.MemberBarCode,
                             NickName = u.NickName,
                             UserType = u.UserType,
                             IntegralTotal =u.IntegralTotal
                         }).FirstOrDefaultAsync();
            return  entity;
        }

        /// <summary>
        /// 分页获取积分详情
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="openId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task<List<IntegralDetailListDto>> GetWXPagedIntegralDetailAsync(int? tenantId, string openId, int pageIndex, int pageSize)
        {
            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                var query = _integraldetailRepository.GetAll().Where(i => i.OpenId == openId);
                var entity = from i in query
                             select new IntegralDetailListDto()
                             {
                                 Id=i.Id,
                                 CreationTime =i.CreationTime,
                                 Integral=i.Integral,
                                 Type = i.Type,
                                 Desc =i.Desc
                             };
                return await entity.OrderByDescending(v=>v.CreationTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
        }
    }
}

