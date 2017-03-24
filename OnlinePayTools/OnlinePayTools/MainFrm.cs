using ComLibrary;
using CryptUtils;
using DBLibrary;
using OnlinePayTools.Properties;
using SandLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace OnlinePayTools
{
    public partial class MainFrm : Form
    {
        //public static bool IsRunning = false;
        LogUtil log = new LogUtil();
        private List<ChannelInfo> channelInfoList = new List<ChannelInfo>() { new ChannelInfo("100001", "杉德支付平台"), new ChannelInfo("100002", "盛迪嘉支付平台") };

        public MainFrm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            //初始化系统数据
            loadInitChannelConfig();
            this.cmbSYS.DataSource = channelInfoList;
            this.cmbSYS.DisplayMember = "SysName";
            this.cmbSYS.ValueMember = "SysId";
            this.cmbSYS.SelectedIndex = 0;
            this.rbSingle.Checked = true;
            this.rbSinglePay.Checked = true;
            //btnRefresh_Click(null, null);
            //this.initSandDFRequestData();

            try
            {
                Settings cfg = Settings.Default;
                DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                string strSelectSql = "select * from ChannelConfig where 1=1 and channelCode='100001'";
                List<MerchantInfo> mchInfoList = dbUtil.QueryForList<MerchantInfo>(strSelectSql, null);
                if (mchInfoList != null && mchInfoList.Count > 0)
                {
                    this.cmbMerchant.DataSource = mchInfoList;
                    this.cmbMerchant.DisplayMember = "mchName";
                    this.cmbMerchant.ValueMember = "mchId";
                    this.cmbMerchant.SelectedIndex = 0;
                    this.cmbPay.DataSource = mchInfoList;
                    this.cmbPay.DisplayMember = "mchName";
                    this.cmbPay.ValueMember = "mchId";
                    this.cmbPay.SelectedIndex = 0;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //窗口加载的时候 开启一个子线程
            Thread thread = new Thread(new ParameterizedThreadStart(Run));
            thread.Start();
        }

        /// <summary>
        /// 主窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
            //Application.Exit();
        }

        /// <summary>
        /// 初始化渠道信息数据
        /// </summary>
        private void loadInitChannelConfig()
        {

            //try
            //{
            //    Settings cfg = Settings.Default;
            //    DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
            //    string strSelectSql = "select * from ChannelConfig where 1=1 and channelCode='100001'";
            //    List<MerchantInfo> mchInfoList = dbUtil.QueryForList<MerchantInfo>(strSelectSql, null);
            //    if (mchInfoList != null && mchInfoList.Count > 0)
            //    {
            //        this.txbMchID.Text = mchInfoList[0].MchId;
            //        this.txbMchName.Text = mchInfoList[0].MchName;
            //        this.txtPFXPath.Text = mchInfoList[0].Code2;
            //        this.txtPFXPwd.Text = mchInfoList[0].Code1;
            //        this.txtCERPath.Text = mchInfoList[0].Code3;
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    log.Write("初始化杉德渠道数错误：" + ex.Message);
            //}

        }

        #region 单笔批量页面控制
        private void rbSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSingle.Checked)
            {
                this.tpSingle.Parent = tcDK;
                this.tpBatch.Parent = null;
                //this.tcDK.SelectedTab = tpSingle;
            }
            else if (rbBatch.Checked)
            {
                this.tpBatch.Parent = tcDK;
                this.tpSingle.Parent = null;
                this.tcDK.SelectedTab = tpBatch;
            }
        }

        private void rbBatch_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSingle.Checked)
            {
                this.tpSingle.Parent = tcDK;
                this.tpBatch.Parent = null;
                //this.tcDK.SelectedTab = tpSingle;
            }
            else if (rbBatch.Checked)
            {
                this.tpBatch.Parent = tcDK;
                this.tpSingle.Parent = null;
                this.tcDK.SelectedTab = tpBatch;
            }

        }

        private void rbSinglePay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSinglePay.Checked)
            {
                this.tpSinglePay.Parent = tcDF;
                this.tpBatchPay.Parent = null;
                //this.tcDK.SelectedTab = tpSingle;
            }
            else if (rbBatchPay.Checked)
            {
                this.tpBatchPay.Parent = tcDF;
                this.tpSinglePay.Parent = null;
                this.tcDF.SelectedTab = tpBatchPay;
            }
        }

        private void rbBatchPay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSinglePay.Checked)
            {
                this.tpSinglePay.Parent = tcDF;
                this.tpBatchPay.Parent = null;
                //this.tcDF.SelectedTab = tpSinglePay;
            }
            else if (rbBatchPay.Checked)
            {
                this.tpBatchPay.Parent = tcDF;
                this.tpSinglePay.Parent = null;
                this.tcDF.SelectedTab = tpBatchPay;
            }
        }

        #endregion

        #region 参数刷新
        /// <summary>
        /// 代扣请求参数刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            initSandDKRequestData();
        }
        /// <summary>
        /// 代付请求参数刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshPay_Click(object sender, EventArgs e)
        {
            this.initSandDFRequestData();
        }
        /// <summary>
        /// 代付请求参数初始化
        /// </summary>
        private void initSandDFRequestData()
        {
            this.dgvPayRequest.DataSource = null;
            this.dgvPayRequest.Rows.Clear();
            SandDFRequestParamList sandParams = new SandDFRequestParamList();
            foreach (KeyValuePair<string, object> kvp in sandParams.GetKeyValue())
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell tbKeyCell = new DataGridViewTextBoxCell();
                string[] keySplitStr = kvp.Key.Split('|');
                if (keySplitStr.Length == 4)//带类型
                {
                    tbKeyCell.Value = keySplitStr[0] + "|" + keySplitStr[1];
                    row.Cells.Add(tbKeyCell);
                    string type = keySplitStr[2];
                    string visble = keySplitStr[3];
                    switch (type)
                    {
                        case "s":
                            DataGridViewTextBoxCell tbValueCell = new DataGridViewTextBoxCell();
                            tbValueCell.Value = kvp.Value;
                            row.Cells.Add(tbValueCell);
                            this.dgvPayRequest.Rows.Add(row);
                            break;
                        case "listDic":
                            DataGridViewComboBoxCell cbKeyCell = new DataGridViewComboBoxCell();
                            cbKeyCell.DataSource = (List<DictionaryEntry>)kvp.Value;
                            cbKeyCell.DisplayMember = "Key";
                            cbKeyCell.ValueMember = "Value";
                            row.Cells.Add(cbKeyCell);
                            this.dgvPayRequest.Rows.Add(row);
                            dgvPayRequest.Rows[cbKeyCell.RowIndex].Cells["DF_VALUE"].Value = ((List<DictionaryEntry>)kvp.Value)[0].Value;
                            break;
                    }
                    if (visble != "Y")
                    {
                        row.Visible = false;
                    }
                }
            }
        }
        private void initSandDKRequestData()
        {
            this.dgvRequestParam.DataSource = null;
            this.dgvRequestParam.Rows.Clear();
            SandDKRequestParamList sandParams = new SandDKRequestParamList();
            foreach (KeyValuePair<string, object> kvp in sandParams.GetKeyValue())
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell tbKeyCell = new DataGridViewTextBoxCell();
                string[] keySplitStr = kvp.Key.Split('|');
                if (keySplitStr.Length == 4)//带类型
                {
                    tbKeyCell.Value = keySplitStr[0] + "|" + keySplitStr[1];
                    row.Cells.Add(tbKeyCell);
                    string type = keySplitStr[2];
                    string visble = keySplitStr[3];
                    switch (type)
                    {
                        case "s":
                            DataGridViewTextBoxCell tbValueCell = new DataGridViewTextBoxCell();
                            tbValueCell.Value = kvp.Value;
                            row.Cells.Add(tbValueCell);
                            this.dgvRequestParam.Rows.Add(row);
                            break;
                        case "listDic":
                            DataGridViewComboBoxCell cbKeyCell = new DataGridViewComboBoxCell();
                            cbKeyCell.DataSource = (List<DictionaryEntry>)kvp.Value;
                            cbKeyCell.DisplayMember = "Key";
                            cbKeyCell.ValueMember = "Value";
                            row.Cells.Add(cbKeyCell);
                            this.dgvRequestParam.Rows.Add(row);
                            dgvRequestParam.Rows[cbKeyCell.RowIndex].Cells["DK_VALUE"].Value = ((List<DictionaryEntry>)kvp.Value)[0].Value;
                            break;
                    }
                    if (visble != "Y")
                    {
                        row.Visible = false;
                    }
                }
            }
        }
        #endregion

        //代付代扣提交
        /// <summary>
        ///选择对接平台
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSYS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 选择代扣商户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbMerchant_SelectedIndexChanged(object sender, EventArgs e)
        {
            MerchantInfo mchInfo = (MerchantInfo)this.cmbMerchant.SelectedItem;
            if (mchInfo != null)
            {
                switch (mchInfo.ChannelCode)
                {
                    case "100001":
                        initSandDKRequestData();
                        break;
                }
            }

        }

        private void cmbPay_SelectedIndexChanged(object sender, EventArgs e)
        {
            MerchantInfo mchInfo = (MerchantInfo)this.cmbMerchant.SelectedItem;
            if (mchInfo != null)
            {
                switch (mchInfo.ChannelCode)
                {
                    case "100001":
                        this.initSandDFRequestData();
                        break;
                }
            }

        }

        #region 单笔提交
        /// <summary>
        /// 代付提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaySubmit_Click(object sender, EventArgs e)
        {
            //获取商户信息

            MerchantInfo mchInfo = (MerchantInfo)this.cmbPay.SelectedItem;
            if (mchInfo == null)
            {
                MessageBox.Show("请选择代付商户");
                return;
            }
            string mchId = mchInfo.MchId;
            string mchName = mchInfo.MchName;
            string pfxPwd = mchInfo.Code1;
            string pfxPath = mchInfo.Code2;
            string cerPath = mchInfo.Code3;
            try
            {
                //定义Json转换类
                JavaScriptSerializer jsonSer = new JavaScriptSerializer();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                int rowCount = this.dgvRequestParam.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (dgvRequestParam.Rows[i].Cells["DK_VALUE"] == null || dgvRequestParam.Rows[i].Cells["DK_VALUE"].Value == null)
                    {
                        MessageBox.Show("请选择" + dgvRequestParam.Rows[i].Cells["DK_NAME"].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = dgvRequestParam.Rows[i].Cells["DK_NAME"].Value.ToString().Trim();
                    string valueField = dgvRequestParam.Rows[i].Cells["DK_VALUE"].Value.ToString().Trim();

                    if (nameField != null && nameField.Split('|').Length == 2)
                    {
                        key = nameField.Split('|')[0];
                        value = valueField;
                        if (key == "tranAmt")
                        {
                            value = (Convert.ToInt64(value) * 100).ToString().PadLeft(12, '0');
                        }
                        dic.Add(key, value);
                    }
                }

                string orderId = "200" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(2) + ComUtils.CreateRandomNum(4);

                //orderId,mchId,mchName,status,amount,bankOrderNo,outOrderId,,createTime,updateTime,respCode,respMsg,remark
                try
                {
                    string payType = "0";
                    if (dic["accAttr"].Equals("0"))
                    {
                        payType = "0";//对私
                        dic.Add("productId", "00000004");
                    }
                    else if (dic["accAttr"].Equals("1"))
                    {
                        payType = "1";//对公
                        dic.Add("productId", "00000003");
                    }

                    //保存记录
                    string strInsertSql = "insert into CollectionOrder(orderId,orderType,mchId,mchName,status,amount,payType,bankOrderNo,outOrderId,createTime,updateTime,respCode,respMsg,remark)"
                           + " VALUES ('" + orderId + "'"
                           + ",'df'"
                           + ",'" + mchId + "'"
                           + ",'" + mchName + "'"
                           + ",0"
                           + "," + Convert.ToInt64(dic["tranAmt"])//转为分存储
                           + ",'" + payType + "'"//对公对私标识
                           + ",''"
                           + ",'" + dic["orderCode"] + "'"
                           + ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                           + ",null"
                           + ",''"
                           + ",''"
                           + ",''"
                           + ")";
                    Settings cfg = Settings.Default;
                    DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                    int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                    //LogUtil.Write("保存成功"+effectNo+"
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    log.Write(ex, MsgType.Error);
                    return;
                }
                //将订单号变为数据库数据，防止订单重复。
                dic.Remove("orderCode");
                dic.Add("orderCode", orderId);

                Dictionary<string, string> dicRslt;
                //解密后的服务器返回
                MessageCryptWorker.trafficMessage resp = AgentPayMessage(dic, mchId, pfxPath, pfxPwd, cerPath);
                //检查验签结果
                log.Write("验签结果" + resp.sign);
                //解析报文，读取业务报文体内具体字段的值
                log.Write(resp.encryptData, MsgType.Information);
                dicRslt = jsonSer.Deserialize<Dictionary<string, string>>(resp.encryptData);
                //dicRslt = (Dictionary<string, string>)JsonUtil.JsonToObject(resp.encryptData, dicRslt);
                string orderCode = dicRslt.ContainsKey("orderCode") ? dicRslt["orderCode"] : "";
                string respCode = dicRslt.ContainsKey("respCode") ? dicRslt["respCode"] : "";
                string respDesc = dicRslt.ContainsKey("respDesc") ? dicRslt["respDesc"] : "";
                string sandSerial = dicRslt.ContainsKey("sandSerial") ? dicRslt["sandSerial"] : "";
                string resultFlag = dicRslt.ContainsKey("resultFlag") ? dicRslt["resultFlag"] : "";
                log.Write("respCode[" + respCode + "]" + "respDesc[" + respDesc + "]");

                string status = "0";

                if ("0000".Equals(respCode))
                {
                    if ("0".Equals(resultFlag))
                    {
                        status = "1";
                    }
                    else if ("1".Equals(resultFlag))
                    {
                        status = "2";
                    }
                }
                try
                {
                    string strInsertSql = "update CollectionOrder set "
                           + " status = " + status
                           + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                           + ", bankOrderNo='" + sandSerial + "'"
                           + ",respCode='" + respCode + "'"
                           + ",respMsg='" + respDesc + "'"
                           + " where orderId='" + orderCode + "'";
                    Settings cfg = Settings.Default;
                    DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                    int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    log.Write(ex, MsgType.Error);
                    return;
                }
                if ("0000".Equals(respCode))
                {
                    if ("0".Equals(resultFlag))
                    {
                        MessageBox.Show("代付完成，成功！");
                    }
                    else if ("1".Equals(resultFlag))
                    {
                        MessageBox.Show("代付失败，错误原因：" + respDesc);
                    }
                    else
                    {
                        MessageBox.Show("代付提交完成，银行处理中");
                    }
                }
                else
                {
                    MessageBox.Show("代付提交异常！错误码：" + respCode + "(" + respDesc + ")");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Write(ex, MsgType.Error);
            }
        }
        /// <summary>
        /// 代扣提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSingleSubmit_Click(object sender, EventArgs e)
        {
            //获取商户信息

            MerchantInfo mchInfo = (MerchantInfo)this.cmbMerchant.SelectedItem;
            if (mchInfo == null)
            {
                MessageBox.Show("请选择代扣商户");
                return;
            }
            string mchId = mchInfo.MchId;
            string mchName = mchInfo.MchName;
            string pfxPwd = mchInfo.Code1;
            string pfxPath = mchInfo.Code2;
            string cerPath = mchInfo.Code3;
            switch (mchInfo.ChannelCode)
            {
                case "100001":
                    doSandDK(this.dgvRequestParam, null, mchId, mchName, pfxPath, pfxPwd, cerPath);
                    break;

            }

        }
        #endregion

        #region 批量提交
        /// <summary>
        /// 选择批量代扣文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                this.txtBatchFile.Text = file;
            }
        }
        /// <summary>
        ///选择代付批量文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectPayFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                this.txbBatchPayFile.Text = file;
            }
        }
        /// <summary>
        /// 批量代扣提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //获取商户信息
                MerchantInfo mchInfo = (MerchantInfo)this.cmbMerchant.SelectedItem;
                if (mchInfo == null)
                {
                    MessageBox.Show("请选择代扣商户");
                    return;
                }
                string mchId = mchInfo.MchId;
                string mchName = mchInfo.MchName;
                string pfxPwd = mchInfo.Code1;
                string pfxPath = mchInfo.Code2;
                string cerPath = mchInfo.Code3;
                //解析文件
                string xlsFile = this.txtBatchFile.Text;
                DataTable dt = OfficeUtils.readExcelToDataTable(xlsFile);
                if (dt != null && dt.Rows.Count >= 1 && dt.Rows[0][0].ToString() != "订单号")
                {
                    throw new Exception("数据模板不对，请选择正确的数据模板。");
                }
                switch (mchInfo.ChannelCode)
                {
                    case "100001":
                        doSandDK(null, dt, mchId, mchName, pfxPath, pfxPwd, cerPath);
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 代付批量提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchPaySubmit_Click(object sender, EventArgs e)
        {

        }
        #endregion

        //商户信息设置
        /// <summary>
        /// 这只pfx文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPFXSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                this.txtPFXPath.Text = file;
            }

        }

        /// <summary>
        /// 设置cer证书路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCERSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                this.txtCERPath.Text = file;
            }

        }

        /// <summary>
        /// 商户信息保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            string sysId = this.cmbSYS.SelectedValue.ToString();
            string sysName = this.cmbSYS.Text;
            string mchId = this.txbMchID.Text;
            string mchName = this.txbMchName.Text;
            string code1 = this.txtPFXPwd.Text;
            string code2 = this.txtPFXPath.Text;
            string code3 = this.txtCERPath.Text;
            try
            {
                Settings cfg = Settings.Default;
                DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                string strSelectSql = "select * from ChannelConfig where 1=1 and channelCode='" + sysId + "' and mchId ='" + mchId + "'";
                DataTable dt = dbUtil.ExecuteDataTable(strSelectSql, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (MessageBox.Show("该商户信息已经存在", "继续保存将覆盖原纪录？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        //删除原纪录
                        string strDeleteSql = "delete from ChannelConfig where channelCode='" + sysId + "' and mchId ='" + mchId + "'";
                        int effectNo = dbUtil.ExecuteNonQuery(strDeleteSql, null);
                    }
                }

                //保存记录
                string strInsertSql = "insert into ChannelConfig(channelCode,channelName,mchId,mchName,status,createTime,updateTime,encryptType,code1,code2,code3,code4,code5,reserve1,reserve2)"
                       + " VALUES ('" + sysId + "'"
                       + ",'" + sysName + "'"
                       + ",'" + mchId + "'"
                       + ",'" + mchName + "'"
                       + ",1"
                       + ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                       + ",null"
                       + ",1"
                       + ",'" + code1 + "'"
                       + ",'" + code2 + "'"
                       + ",'" + code3 + "'"
                       + ",''"
                       + ",''"
                       + ",''"
                       + ",''"
                       + ")";

                int insertCount = dbUtil.ExecuteNonQuery(strInsertSql, null);
                if (insertCount == 1)
                {
                    MessageBox.Show("保存成功!");
                }
                //重新load商户信息
                try
                {
                    strSelectSql = "select * from ChannelConfig where 1=1 and channelCode='" + sysId + "'";
                    List<MerchantInfo> mchInfoList = dbUtil.QueryForList<MerchantInfo>(strSelectSql, null);
                    if (mchInfoList != null && mchInfoList.Count > 0)
                    {
                        this.cmbMerchant.DataSource = null;
                        this.cmbMerchant.Items.Clear();
                        this.cmbMerchant.DataSource = mchInfoList;
                        this.cmbMerchant.DisplayMember = "mchName";
                        this.cmbMerchant.ValueMember = "mchId";
                        this.cmbPay.DataSource = null;
                        this.cmbPay.Items.Clear();
                        this.cmbPay.DataSource = mchInfoList;
                        this.cmbPay.DisplayMember = "mchName";
                        this.cmbPay.ValueMember = "mchId";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        /// <summary>
        /// 渠道信息查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQueryChannel_Click(object sender, EventArgs e)
        {
            try
            {
                String sqlId = "select * from ChannelConfig c where c.channelCode='" + this.cmbSYS.SelectedValue.ToString() + "'";
                Settings cfg = Settings.Default;
                DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                DataTable dt = dbUtil.ExecuteDataTable(sqlId, null);
                this.dgvChannelInfo.DataSource = dt;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //订单查询
        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlId = "SELECT "
                                    + "o.orderId     as 请求订单号,"
                                    + "iif(o.orderType='df','代付',iif(o.orderType='dk','代扣')) as 订单类型,"
                                    + "o.mchId       as 渠道商户号,"
                                    + "iif(o.status='0','进行中',iif(o.status='1','成功',iif(o.status='2','失败'))) as 订单状态,"
                                    + "o.amount/100  as 订单金额,"
                                    + "o.mchName     as 渠道商户名称,"
                                    + "iif(o.payType=0,'对私',iif(o.payType=1,'对公'))  as 对公对私标示,"
                                    + "o.createTime  as 订单时间,"
                                    + "o.updateTime  as 完成时间,"
                                    + "o.bankOrderNo as 渠道订单号,"
                                    + "o.outOrderId  as 原始订单号,"
                                    + "o.respCode    as 渠道返回码,"
                                    + "o.respMsg     as 渠道返回描"
                                   + " FROM CollectionOrder o order by o.createTime desc";
                Settings cfg = Settings.Default;
                DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                DataTable dt = dbUtil.ExecuteDataTable(sqlId, null);
                dgv_sd.DataSource = dt;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //查询独立进程
        /// <summary>
        /// 订单自动查询进程
        /// </summary>
        /// <param name="obj"></param>
        private void Run(object obj)
        {
            while (true)
            {
                Thread.Sleep(1000 * 60 * 5);//每5分钟运行一次

                try
                {
                    //获取商户信息
                    string sqlStr = "select * from CollectionOrder c inner join ChannelConfig m on c.mchId=m.mchId  where 1=1 and c.status='0' and c.createTime >=#" + DateTime.Now.AddMinutes(-60) + "# and c.createTime <=#" + DateTime.Now.AddMinutes(-5) + "#";
                    //string sqlStr = "select * from CollectionOrder c inner join ChannelConfig m on c.mchId=m.mchId  where 1=1 and c.createTime >=#" + DateTime.Now.AddMinutes(-30) + "# and c.createTime <=#" + DateTime.Now.AddMinutes(-1) + "#";
                    Settings cfg = Settings.Default;
                    DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                    DataTable dt = dbUtil.ExecuteDataTable(sqlStr, null);
                    if (null != dt && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string sysId = dt.Rows[i]["channelCode"].ToString();
                            string orderId = dt.Rows[i]["orderId"].ToString();
                            string mchId = dt.Rows[i]["m.mchId"].ToString();
                            string mchName = dt.Rows[i]["m.mchName"].ToString();
                            string pfxPath = dt.Rows[i]["code2"].ToString();
                            string pfxPwd = dt.Rows[i]["code1"].ToString();
                            string cerPath = dt.Rows[i]["code3"].ToString();
                            string payType = dt.Rows[i]["payType"].ToString();
                            string orderType = dt.Rows[i]["orderType"].ToString();
                            string transTime = dt.Rows[i]["c.createTime"].ToString();
                            switch (sysId)
                            {
                                case "100001":
                                    doSandQuery(orderId, mchId,transTime, mchName, pfxPath, pfxPwd, cerPath, payType, orderType);
                                    break;

                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    log.Write(ex, MsgType.Error);
                }
            }
        }

        #region 杉德支付平台

        /// <summary>
        /// 杉德代扣处理
        /// </summary>
        /// <param name="RequestParamDGV"></param>
        /// <param name="dt"></param>
        /// <param name="mchId"></param>
        /// <param name="mchName"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        private void doSandDK(DataGridView RequestParamDGV, DataTable dt, string mchId, string mchName, string pfxPath, string pfxPwd, string cerPath)
        {
            //定义Json转换类
            List<Dictionary<string, string>> batchDKparams = new List<Dictionary<string, string>>();
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            if (dt == null)
            { //单笔
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                int rowCount = this.dgvRequestParam.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (dgvRequestParam.Rows[i].Cells["DK_VALUE"] == null || dgvRequestParam.Rows[i].Cells["DK_VALUE"].Value == null)
                    {
                        MessageBox.Show("请选择" + dgvRequestParam.Rows[i].Cells["DK_NAME"].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = dgvRequestParam.Rows[i].Cells["DK_NAME"].Value.ToString().Trim();
                    string valueField = dgvRequestParam.Rows[i].Cells["DK_VALUE"].Value.ToString().Trim();

                    if (nameField != null && nameField.Split('|').Length == 2)
                    {
                        key = nameField.Split('|')[0];
                        value = valueField;
                        if (key == "tranAmt")
                        {
                            int amount = 0;
                            if (!int.TryParse(value, out amount))
                            {
                                MessageBox.Show("代扣金额格式不正确");
                                return;
                            }
                            value = (Convert.ToInt64(amount) * 100).ToString().PadLeft(12, '0');
                        }
                        tempDic.Add(key, value);
                    }
                }
                batchDKparams.Add(tempDic);
            }
            else if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    Dictionary<string, string> tempDic = new Dictionary<string, string>();
                    tempDic.Add("version", "01");
                    //tempDic.Add("productId", "00000002");
                    tempDic.Add("tranTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    tempDic.Add("orderCode", dt.Rows[i][0].ToString());
                    int amount = 0;
                    if (!int.TryParse(dt.Rows[i][1].ToString(), out amount))
                    {
                        MessageBox.Show("第" + (i + 1) + "行代扣金额格式不正确");
                        return;
                    }
                    string amountStr = (Convert.ToInt64(amount) * 100).ToString().PadLeft(12, '0');
                    tempDic.Add("tranAmt", amountStr);
                    tempDic.Add("currencyCode", "156");
                    tempDic.Add("accAttr", dt.Rows[i][2].ToString());
                    tempDic.Add("accType", dt.Rows[i][3].ToString());
                    tempDic.Add("accNo", dt.Rows[i][4].ToString());
                    tempDic.Add("accName", dt.Rows[i][5].ToString());
                    tempDic.Add("bankName", "");
                    tempDic.Add("provNo", "010000");
                    tempDic.Add("cityNo", "");
                    tempDic.Add("certType", dt.Rows[i][6].ToString());
                    tempDic.Add("certNo", dt.Rows[i][7].ToString());
                    tempDic.Add("cardId", dt.Rows[i][8].ToString());
                    tempDic.Add("phone", dt.Rows[i][9].ToString());
                    tempDic.Add("bankInsCode", "");
                    tempDic.Add("purpose", "collection");
                    tempDic.Add("reqReserved", "");
                    tempDic.Add("extend", "");
                    batchDKparams.Add(tempDic);
                }
            }
            try
            {
                int count = 0;
                List<string> successRecord = new List<string>();
                List<string> errorRecord = new List<string>();
                List<string> processingRecord = new List<string>();
                foreach (Dictionary<string, string> dic in batchDKparams)
                {
                    count++;
                    string orderId = "200" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(2) + ComUtils.CreateRandomNum(4);
                    //orderId,mchId,mchName,status,amount,bankOrderNo,outOrderId,,createTime,updateTime,respCode,respMsg,remark
                    try
                    {
                        string payType = "0";
                        if (dic["accAttr"].Equals("0"))
                        {
                            payType = "0";//对私
                            dic.Add("productId", "00000002");
                        }
                        else if (dic["accAttr"].Equals("1"))
                        {
                            payType = "1";//对公
                            dic.Add("productId", "00000001");
                        }

                        //杉德的tranTime需要用于查询    
                        DateTime tranDt = DateTime.ParseExact(dic["tranTime"], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        //DateTime tranDt = Convert.ToDateTime(dic["tranTime"], IFormatProvider);

                        //保存记录
                        string strInsertSql = "insert into CollectionOrder(orderId,orderType,mchId,mchName,status,amount,payType,bankOrderNo,outOrderId,createTime,updateTime,respCode,respMsg,remark)"
                               + " VALUES ('" + orderId + "'"
                               + ",'dk'"
                               + ",'" + mchId + "'"
                               + ",'" + mchName + "'"
                               + ",0"
                               + "," + Convert.ToInt64(dic["tranAmt"])//转为分存储
                               + ",'" + payType + "'"//对公对私标识
                               + ",''"
                               + ",'" + dic["orderCode"] + "'"
                               + ",'" + tranDt.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ",null"
                               + ",''"
                               + ",''"
                               + ",''"
                               + ")";
                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                        //LogUtil.Write("保存成功"+effectNo+"
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        log.Write(ex, MsgType.Error);
                        return;
                    }
                    //将订单号变为数据库数据，防止订单重复。
                    dic.Remove("orderCode");
                    dic.Add("orderCode", orderId);

                    Dictionary<string, string> dicRslt;
                    //解密后的服务器返回
                    MessageCryptWorker.trafficMessage resp = CollectionMessage(dic, mchId, pfxPath, pfxPwd, cerPath);
                    //检查验签结果
                    log.Write("验签结果" + resp.sign);
                    //解析报文，读取业务报文体内具体字段的值
                    log.Write(resp.encryptData, MsgType.Information);
                    dicRslt = jsonSer.Deserialize<Dictionary<string, string>>(resp.encryptData);
                    //dicRslt = (Dictionary<string, string>)JsonUtil.JsonToObject(resp.encryptData, dicRslt);
                    string orderCode = dicRslt.ContainsKey("orderCode") ? dicRslt["orderCode"] : "";
                    string respCode = dicRslt.ContainsKey("respCode") ? dicRslt["respCode"] : "";
                    string respDesc = dicRslt.ContainsKey("respDesc") ? dicRslt["respDesc"] : "";
                    string sandSerial = dicRslt.ContainsKey("sandSerial") ? dicRslt["sandSerial"] : "";
                    string resultFlag = dicRslt.ContainsKey("resultFlag") ? dicRslt["resultFlag"] : "";
                    log.Write("respCode[" + respCode + "]" + "respDesc[" + respDesc + "]");

                    string status = "0";

                    if ("0000".Equals(respCode))
                    {
                        if ("0".Equals(resultFlag))
                        {
                            status = "1";
                        }
                        else if ("1".Equals(resultFlag))
                        {
                            status = "2";
                        }
                    }
                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + sandSerial + "'"
                               + ",respCode='" + respCode + "'"
                               + ",respMsg='" + respDesc + "'"
                               + " where orderId='" + orderCode + "'";
                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        log.Write(ex, MsgType.Error);
                        return;
                    }
                    if ("0000".Equals(respCode))
                    {
                        if ("0".Equals(resultFlag))
                        {
                            successRecord.Add(orderCode);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣完成，成功！");
                            }
                        }
                        else if ("1".Equals(resultFlag))
                        {
                            errorRecord.Add(orderCode);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣失败，错误原因：" + respDesc);
                            }
                        }
                        else
                        {
                            processingRecord.Add(orderCode);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(orderCode);
                        if (count == batchDKparams.Count)
                        {
                            MessageBox.Show("代扣提交异常！错误码：" + respCode + "(" + respDesc + ")");
                        }
                    }
                }
                if (1 != batchDKparams.Count)
                {
                    MessageBox.Show("代扣完成，共" + batchDKparams.Count + "笔，成功：" + successRecord.Count + "笔，进行中：" + processingRecord.Count + "笔，失败或异常：" + errorRecord.Count + "笔。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Write(ex, MsgType.Error);
            }
        }

        /// <summary>
        /// 杉德查询补单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchName"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        /// <param name="payType"></param>
        private void doSandQuery(string orderId, string mchId,string tranTime, string mchName, string pfxPath, string pfxPwd, string cerPath, string payType, string orderType)
        {
            Settings cfg = Settings.Default;
            DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("version", "01");
            if (payType == "0" && orderType=="dk")
            {
                dic.Add("productId", "00000002");// 代收对公    00000001 代收对私    00000002  代付对私    00000004 
            }
            else if (payType == "1" && orderType=="dk")
            {
                dic.Add("productId", "00000001");// 代收对公    00000001 代收对私    00000002  代付对私    00000004 
            }
            else if(payType == "0" && orderType=="df")
            {
                dic.Add("productId", "00000004");
            }
            else if (payType == "1" && orderType == "df")
            {
                dic.Add("productId", "00000003");
            }
            //DateTime tranDt = DateTime.ParseExact(dic["tranTime"], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
           
            DateTime datetime;
            if(!DateTime.TryParse(tranTime,out datetime)){
                log.Write(orderId + "--时间格式错误，查询失败");
                return;
            }
            dic.Add("tranTime", datetime.ToString("yyyyMMddHHmmss"));
            dic.Add("orderCode", orderId);
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();

            Dictionary<string, string> dicRslt;
            //解密后的服务器返回
            MessageCryptWorker.trafficMessage resp = QueryOrderMessage(dic, pfxPath, pfxPwd, cerPath, mchId);
            //检查验签结果
            log.Write("验签结果" + resp.sign);
            //解析报文，读取业务报文体内具体字段的值
            log.Write(resp.encryptData, MsgType.Information);
            dicRslt = jsonSer.Deserialize<Dictionary<string, string>>(resp.encryptData);
            //dicRslt = (Dictionary<string, string>)JsonUtil.JsonToObject(resp.encryptData, dicRslt);
            string orderCode = dicRslt.ContainsKey("orderCode") ? dicRslt["orderCode"] : "";
            string respCode = dicRslt.ContainsKey("respCode") ? dicRslt["respCode"] : "";
            string respDesc = dicRslt.ContainsKey("respDesc") ? dicRslt["respDesc"] : "";
            string sandSerial = dicRslt.ContainsKey("sandSerial") ? dicRslt["sandSerial"] : "";
            string resultFlag = dicRslt.ContainsKey("resultFlag") ? dicRslt["resultFlag"] : "";
            log.Write("respCode[" + respCode + "]" + "respDesc[" + respDesc + "]");
            string status = "0";

            if ("0000".Equals(respCode))
            {
                if ("0".Equals(resultFlag))
                {
                    status = "1";
                }
                else if ("1".Equals(resultFlag))
                {
                    status = "2";
                }
            }
            try
            {
                string strUpdateSql = "update CollectionOrder set "
                       + " status = " + status
                       + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                       + ", bankOrderNo='" + sandSerial + "'"
                       + ",respCode='" + respCode + "'"
                       + ",respMsg='" + respDesc + "'"
                       + " where orderId='" + orderCode + "'";
                cfg = Settings.Default;
                dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                int effectNo = dbUtil.ExecuteNonQuery(strUpdateSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Write(ex, MsgType.Error);
                return;
            }
        }
        /// <summary>
        ///代扣提交
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="mchId"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        /// <returns></returns>
        private MessageCryptWorker.trafficMessage CollectionMessage(Dictionary<string, string> dic, string mchId, string pfxPath, string pfxPwd, string cerPath)
        {

            //报文结构体初始化
            MessageCryptWorker.trafficMessage msgRequestSource = new MessageCryptWorker.trafficMessage();
            //dic.Add("version", "01");
            //dic.Add("cityNo", "010000");
            //dic.Add("certType", "0001");
            //dic.Add("productId", "00000002");
            //dic.Add("purpose", "collection");
            //dic.Add("accNo", "6226220209634996");
            //dic.Add("accName", "TEST");
            //dic.Add("bankInsCode", "48270000");
            //dic.Add("bankName", "世界银行");
            //dic.Add("accAttr", "0");
            //dic.Add("timeOut", "20161115123021");
            //dic.Add("certNo", "321281198702253717");
            //dic.Add("tranTime", "20161114123021");
            //dic.Add("provNo", "010000");
            //dic.Add("phone", "12345678901");
            //dic.Add("cardId", "321281198702253717");
            //dic.Add("tranAmt", "000000000100");
            //dic.Add("orderCode", "201611131000001042");
            //dic.Add("accType", "4");
            //dic.Add("currencyCode", "156");



            //发送类实体化
            MessageCryptWorker worker = new MessageCryptWorker();
            worker.EncodeCode = Encoding.UTF8; //encoding 使用utf8
            worker.PFXFile = pfxPath; //商户pfx证书路径
            worker.PFXPassword = pfxPwd;  //商户pfx证书密码
            worker.CerFile = cerPath; //杉德cer证书路径
            //string ServerUrl = "http://61.129.71.103:7970/agent-main/openapi/collection";
            string ServerUrl = "https://caspay.sandpay.com.cn/agent-main/openapi/collection";
            msgRequestSource.merId = mchId; //商户号
            msgRequestSource.transCode = "RTCO";        //交易代码
            msgRequestSource.extend = "";               //扩展域

            //报文体json
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            msgRequestSource.encryptData = jsonSer.Serialize(dic);
            //msgRequestSource.encryptData = JsonUtil.ObjectToJson(dic);
            //encrytpKey会在发送前加密时自动生成16位的随机字符

            log.Write("待发送报文为：" + msgRequestSource.encryptData);

            MessageCryptWorker.trafficMessage respMessage = worker.postMessage(ServerUrl, msgRequestSource);
            log.Write("服务器返回为：" + respMessage.encryptData);
            return respMessage;

        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="pfxFilePath"></param>
        /// <param name="pfxPassword"></param>
        /// <param name="cerFilePath"></param>
        /// <param name="merId"></param>
        /// <returns></returns>
        private MessageCryptWorker.trafficMessage QueryOrderMessage(Dictionary<string, string> dic, string pfxFilePath,
                string pfxPassword,
                string cerFilePath,
                string merId)
        {
            //报文结构体初始化
            MessageCryptWorker.trafficMessage msgRequestSource = new MessageCryptWorker.trafficMessage();
            //发送类实体化
            MessageCryptWorker worker = new MessageCryptWorker();
            worker.EncodeCode = Encoding.UTF8; //encoding 使用utf8

            worker.PFXFile = pfxFilePath; //商户pfx证书路径
            worker.PFXPassword = pfxPassword;  //商户pfx证书密码
            worker.CerFile = cerFilePath; //杉德cer证书路径


            msgRequestSource.merId = merId; //商户号
            msgRequestSource.transCode = "ODQU";        //交易代码
            msgRequestSource.extend = "";               //扩展域

            //报文体json
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            msgRequestSource.encryptData = jsonSer.Serialize(dic);
            //encrytpKey会在发送前加密时自动生成16位的随机字符

            log.Write("待发送报文为：" + msgRequestSource.encryptData);
            //string ServerUrl = "http://61.129.71.103:7970/agent-main/openapi/queryOrder";
            string ServerUrl = "https://caspay.sandpay.com.cn/agent-main/openapi/queryOrder";

            MessageCryptWorker.trafficMessage respMessage = worker.postMessage(ServerUrl, msgRequestSource);

            log.Write("服务器返回为：" + respMessage.encryptData);
            return respMessage;
        }

        /// <summary>
        /// 代付接口提交
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="pfxFilePath"></param>
        /// <param name="pfxPassword"></param>
        /// <param name="cerFilePath"></param>
        /// <param name="merId"></param>
        /// <returns></returns>
        private MessageCryptWorker.trafficMessage AgentPayMessage(Dictionary<string, string> dic, string pfxFilePath,
        string pfxPassword,
        string cerFilePath,
        string merId)
        {
            //dic.Add("version", "01");
            //dic.Add("productId", "00000003");// 代收对公    00000001 代收对私    00000002  代付对私    00000004 
            //dic.Add("tranTime", "20161114123021");
            //dic.Add("orderCode", "20161113000000001038");
            //dic.Add("timeOut", "20161115123021");
            //dic.Add("tranAmt", "000000000001");
            //dic.Add("currencyCode", "156");
            //dic.Add("accAttr", "0");
            //dic.Add("accType", "2");
            //dic.Add("accNo", "6216261000000000018");
            //dic.Add("accName", "啊啊");
            //dic.Add("bankName", "aaa");
            //dic.Add("bankType", "1234567890");
            //dic.Add("remark", "pay");
            //dic.Add("reqReserved", "");
            //dic.Add("noticeUrl", "");
            //dic.Add("extend", "");
            //报文结构体初始化
            MessageCryptWorker.trafficMessage msgRequestSource = new MessageCryptWorker.trafficMessage();
            //发送类实体化
            MessageCryptWorker worker = new MessageCryptWorker();
            worker.EncodeCode = Encoding.UTF8; //encoding 使用utf8

            worker.PFXFile = pfxFilePath; //商户pfx证书路径
            worker.PFXPassword = pfxPassword;  //商户pfx证书密码
            worker.CerFile = cerFilePath; //杉德cer证书路径


            msgRequestSource.merId = merId; //商户号
            msgRequestSource.transCode = "RTPM";        //交易代码
            msgRequestSource.extend = "";               //扩展域

            //报文体json
            JavaScriptSerializer jsonSer = new JavaScriptSerializer();
            msgRequestSource.encryptData = jsonSer.Serialize(dic);
            //encrytpKey会在发送前加密时自动生成16位的随机字符


            log.Write("待发送报文为：" + msgRequestSource.encryptData);
            //string ServerUrl = "http://61.129.71.103:7970/agent-main/openapi/agentpay";
            string ServerUrl = "https://caspay.sandpay.com.cn/agent-main/openapi/agentpay";

            MessageCryptWorker.trafficMessage respMessage = worker.postMessage(ServerUrl, msgRequestSource);
            log.Write("服务器返回为：" + respMessage.encryptData);

            return respMessage;
        }
        #endregion

    }
}
