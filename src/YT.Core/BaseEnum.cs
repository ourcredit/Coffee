﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using YT.Managers.Users;

namespace YT
{
    /// <summary>
    /// 组织机构类型
    /// </summary>
   public  enum  OrganizationType
    {
        /// <summary>
        /// 政府
        /// </summary>
        Government=1,
        /// <summary>
        /// 企业
        /// </summary>
        Enterprise=2
    }
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 余额支付
        /// </summary>
        BalancePay=1,
        /// <summary>
        /// 在线支付
        /// </summary>
        LinePay=2,
        /// <summary>
        /// 充值
        /// </summary>
        PayCharge=3,
        /// <summary>
        /// 活动支付
        /// </summary>
        ActivityPay=4
    }
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 冰山
        /// </summary>
        Ice =2,
        /// <summary>
        /// 基诺
        /// </summary>
        Jack = 1
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static class AbpSessionExtensions
    {

        /// <summary>
        /// 获取当前登陆用户
        /// </summary>
        /// <param name="abpSession"></param>
        /// <returns></returns>
        public static async Task<User> Current(this IAbpSession abpSession)
        {

            var userRepository = IocManager.Instance.Resolve<IRepository<User, long>>();
            var u = await userRepository.FirstOrDefaultAsync(abpSession.GetUserId());
            if (u == null)
            {
                throw new UserFriendlyException("当前用户未登陆");
            }
            return u;
        }

    }

    /// <summary>
    /// 树结构
    /// </summary>
    public class TreeModel
    {
        public TreeModel() { }

        public TreeModel(int id, string name, string url, string icon = "", IList<TreeModel> childs = null)
        {
            Id = id;
            DisplayName = name;
            Url = url;
            IconUrl = icon;
            Children = childs;
        }
        /// <summary>
        /// key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public IList<TreeModel> Children { get; set; }
    }
}
