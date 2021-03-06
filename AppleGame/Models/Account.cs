///////////////////////////////////////////////////////////
//  Account.cs
//  Implementation of the Class Account
//  Generated by Enterprise Architect
//  Created on:      16-11月-2016 19:47:10
//  Original author: ant_d
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;



using AntDesigner.GameCityBase.interFace;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AntDesigner.GameCityBase
{
    /// <summary>
    /// 玩家账户
    /// </summary>
    public class Account
    {
        public int Id
        {
            get;
            set;
        }
        /// <summary>
        /// 余额
        /// </summary>
        [ConcurrencyCheck]
        public decimal Balance { get; set; }
        /// <summary>
        /// 微信openid
        /// </summary>
        public string WeixinName { get; set; }
        /// <summary>
        /// 账户变动明细
        /// </summary>
        public IList<AccountDetail> AccountDetails
        {
            get; set;
        }
        public Account()
        {

        }
        /// <summary>
        /// 余额变动事件
        /// </summary>
        public event EventHandler AccounBalanceChanged;
        public Account(string name)
        {
            WeixinName = name;
        }
        /// <summary>
        /// 改变余额
        /// </summary>
        /// <param name="amount">变动金额</param>
        /// <param name="explain">变动原因</param>
        public void Addmount(decimal amount, string explain)
        {
            if (amount < 0 && (Balance - amount) < 0)
            {
                throw new ArgumentOutOfRangeException("余额不足够!");
            }
            Balance = Balance + amount;
            AccountDetail accountDetail = new AccountDetail()
            {
                Amount = amount,
                Explain = explain
            };
            AccountDetails.Add(accountDetail);
            if (explain != "赢" || explain != "下注")
            {
                AccounBalanceChanged?.Invoke(this, new ChangeDetail(accountDetail));
            }
            return;
        }
        /// <summary>
        /// 余额变动事件参数
        /// </summary>
        public class ChangeDetail : EventArgs
        {
            public ChangeDetail(AccountDetail accoutDetail_)
            {
                AccoutDetail = accoutDetail_;
            }
            public AccountDetail AccoutDetail { get; set; }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime
        {
            get; set;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifyTime
        { get; set; }
    }//end Account

}//end namespace AntDesigner.AppleGame