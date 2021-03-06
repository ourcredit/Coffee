﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Logging;
using Newtonsoft.Json;
using YT.Configuration;
using YT.Models;

namespace YT.ThreeData
{
    /// <summary>
    /// 后台同步数据
    /// </summary>
    public class BackgroundManager : IDomainService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<StoreOrder> _userOrderRepository;
        private readonly IRepository<Warn> _warnRepository;
     /// <summary>
     /// ctor
     /// </summary>
     /// <param name="orderRepository"></param>
     /// <param name="warnRepository"></param>
     /// <param name="userOrderRepository"></param>
        public BackgroundManager(IRepository<Order> orderRepository, IRepository<Warn> warnRepository,
            IRepository<StoreOrder> userOrderRepository)
        {
            _orderRepository = orderRepository;
            _warnRepository = warnRepository;
            _userOrderRepository = userOrderRepository;
        }

        /// <summary>
        /// test
        /// </summary>
        public async Task GenderOrder()
        {
            var p =
         new QueryParams("CURRENT");
            var t =
            p.ReturnParams();
            var result = HttpHandler.Post(p.Url + "?" + t);
            LogHelper.Logger.Warn(" url+"+ p.Url + "?" + t + " ------------------------------"+ result);
            var temp = JsonConvert.DeserializeObject<ResultItem>(result);
            await InsertOrder(temp.Record);
        }
        /// <summary>
        /// test
        /// </summary>
        public async Task GenderTodayOrder()
        {
            var start = DateTime.Now;
            var left = DateTime.Now.AddHours(-23);
            var p =
         new QueryParams("SPECIFY",left.ToString("yyyyMMddHHmmss"), start.ToString("yyyyMMddHHmmss"));
            var t =
            p.ReturnParams();
            var result = HttpHandler.Post(p.Url + "?" + t);
         //   LogHelper.Logger.Warn(" url+" + p.Url + "?" + t + " ------------------------------" + result);
            var temp = JsonConvert.DeserializeObject<ResultItem>(result);
            await InsertOrder(temp.Record);
        }
        /// <summary>
        /// 同步未成功订单状态为失败
        /// </summary>
        /// <returns></returns>
        public async Task GenderOrderState()
        {
            var sql = @"UPDATE storeorder
SET OrderState = 0
WHERE
	PayState = 1
AND OrderState IS NULL
AND OrderType = 1
AND CreationTime <= SUBDATE(NOW(), INTERVAL 3 MINUTE)";
            var result = MySqlHelper.ExecuteNonQuery(sql);
        }
      
        /// <summary>
        /// 获取报警数据
        /// </summary>
        /// <returns></returns>
        public async Task GenderWarning()
        {

            var p =
         new QueryParams("WARNING");
            var t =
            p.ReturnParams();
            var result = HttpHandler.Post(p.Warn + "?" + t);
            var temp = JsonConvert.DeserializeObject<ResultItem>(result);
            await InsertWarn(temp.Record);
        }
        /// <summary>
        /// test
        /// </summary>
        public async Task Test()
        {
            var url =
                "http://39.104.57.0:8079/QUERY?QID=100003&QLEVEL=HISTORY&USERNAME=user&MONTH=201702&MAC=33a0fd9aa421b45aaafc4a0f39398109";
            var result = HttpHandler.Post(url);
            var temp = JsonConvert.DeserializeObject<ResultItem>(result);
            await InsertOrder(temp.Record);
        }
        /// <summary>
        /// 插入报警数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task InsertWarn(dynamic list)
        {
            var warns = await _warnRepository.GetAllListAsync(c =>!c.State);
            foreach (var item in list)
            {
                if (item[0] == null || item[1] == null || item[2] == null || item[3] == null) continue;
                if (warns.Any(c => c.DeviceNum.Equals(item[0].ToString())&&c.WarnContent.Equals(item[2].ToString()))) continue;
                var war = new Warn()
                {
                    DeviceNum = item[0].ToString(),
                    State = false,
                    WarnContent = item[2].ToString(),
                    WarnNum = item[1].ToString(),
                    WarnTime=DateTime.Parse(item[3].ToString())
                };

                await _warnRepository.InsertAsync(war);
            }
        }
        /// <summary>
        /// 插入订单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task InsertOrder(dynamic list)
        {
            if(list==null) return;
            var left = DateTime.Now.AddDays(-1);
            var orders = await _orderRepository.GetAllListAsync(c=>c.Date>=left);
            foreach (var item in list)
            {
                if (item[1] == null || item[3] == null || item[4] == null) continue;
                if (orders.Any(c => c.OrderNum.Equals(item[1].ToString()))) continue;
                var order = new Order()
                {
                    DeviceNum = item[0].ToString(),
                    OrderNum = item[1].ToString(),
                    ProductNum = int.Parse(item[2].ToString()),
                    Price = double.Parse(item[3].ToString()),
                    PayType = item[4].ToString(),
                    Date = DateTime.Parse(item[5].ToString())
                };
                order.Date = order.Date.AddHours(-8);
                 _orderRepository.Insert(order);
               

            }
        }


    }
    /// <summary>
    /// 
    /// </summary>
    public class ResultItem
    {
        /// <summary>
        /// item
        /// </summary>
        public dynamic[][] Record { get; set; }
        /// <summary>
        /// state
        /// </summary>
        public string Status { get; set; }
    }
    /// <summary>
    /// 参数校验
    /// </summary>
    public class QueryParams
    {
        /// <summary>
        /// 
        /// </summary>
        public QueryParams() { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="month"></param>
        public QueryParams(string type, string start = "", string end = "", string month = "")
        {
            Qlevel = type;
            StartTime = start;
            EndTime = end;
            Month = month;
            Mac = Gender();
        }

        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; } = "http://39.104.57.0:8079/QUERY";
        /// <summary>
        /// 警告
        /// </summary>
        public string Warn { get; set; } = "http://39.104.57.0:8079/WARNING";
        /// <summary>
        /// qid
        /// </summary>
        public string Qid { get; set; } = new Random().Next(100000, 1000000).ToString();

        /// <summary>
        /// usernmae
        /// </summary>
        public string Username { get; set; } = "shuoyibuer";
        /// <summary>
        /// 级别
        /// </summary>
        public string Qlevel { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 截至时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// password
        /// </summary>
        public string Password { get; set; } = "coffee888";
        /// <summary>
        /// 加密
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public string ReturnParams()
        {
            var temp = $"{nameof(Qid).ToUpper()}={Qid}&{nameof(Username).ToUpper()}={Username}" +
                        $"&{nameof(Qlevel).ToUpper()}={Qlevel}&{nameof(Mac).ToUpper()}={Gender()}";
            if (!string.IsNullOrWhiteSpace(StartTime))
            {
                temp += $"&{nameof(StartTime).ToUpper()}={StartTime}";
            }
            if (!string.IsNullOrWhiteSpace(EndTime))
            {
                temp += $"&{nameof(EndTime).ToUpper()}={EndTime}";
            }
            if (!string.IsNullOrWhiteSpace(Month))
            {
                temp += $"&{nameof(Month).ToUpper()}={Month}";
            }
            return temp;
        }
        private string Gender()
        {
            string temp = string.Empty;
            if (Qlevel.Equals("CURRENT") || Qlevel.Equals("SPECIFY") || Qlevel.Equals("WARNING"))
            {
                temp = $"{nameof(Qid).ToUpper()}={Qid}&{nameof(Username).ToUpper()}={Username}" +
                       $"&{nameof(Password).ToUpper()}={Password}&{nameof(Qlevel).ToUpper()}={Qlevel}";
            }
            else
            {
                temp = $"{nameof(Qid).ToUpper()}={Qid}&{nameof(Username).ToUpper()}={Username}" +
                       $"&{nameof(Password).ToUpper()}={Password}&{nameof(Qlevel).ToUpper()}={Qlevel}&{nameof(Month).ToUpper()}={Month}";
            }
            return ToMd5(temp);

        }
        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <returns>加密后的字符串</returns>
        public static string ToMd5(string str)
        {
            byte[] sor = Encoding.UTF8.GetBytes(str);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
    }
}
