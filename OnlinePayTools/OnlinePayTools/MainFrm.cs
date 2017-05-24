using ComLibrary;
using CryptUtils;
using DBLibrary;
using OnlinePayTools.Properties;
using SandLibrary;
using SdjLibrary;
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
using YytLibrary;

namespace OnlinePayTools
{
    public partial class MainFrm : Form
    {
        //public static bool IsRunning = false;
        LogUtil log = new LogUtil();
        private List<ChannelInfo> channelInfoList = new List<ChannelInfo>() { new ChannelInfo("100001", "杉德支付平台"), new ChannelInfo("100002", "盛迪嘉支付平台"), new ChannelInfo("100003", "银盈通支付平台") };

        public MainFrm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            //初始化系统数据
            //loadInitChannelConfig();
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
                string strSelectSql = "select * from ChannelConfig where 1=1";
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

            //隐藏代付
            //this.tabPage2.Parent = null;
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
            //获取商户信息
            MerchantInfo mchInfo = (MerchantInfo)this.cmbMerchant.SelectedItem;
            if (mchInfo != null)
            {
                switch (mchInfo.ChannelCode)
                {
                    case "100001":
                        BaseRequestParamList sandDKRequestData = new SandDKRequestParamList();
                        initRequestData(sandDKRequestData, this.dgvRequestParam);
                        break;
                    case "100002":
                        BaseRequestParamList sdjDKRequestData = new SdjDKRequestParamList();
                        initRequestData(sdjDKRequestData, this.dgvRequestParam);
                        break;
                    case "100003":
                        BaseRequestParamList yytDKRequestData = new YytDKRequestParamList();
                        initRequestData(yytDKRequestData, this.dgvRequestParam);
                        break;
                }
            }
        }
        /// <summary>
        /// 代付请求参数刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshPay_Click(object sender, EventArgs e)
        {
            //获取商户信息
            MerchantInfo mchInfo = (MerchantInfo)this.cmbMerchant.SelectedItem;
            if (mchInfo != null)
            {
                switch (mchInfo.ChannelCode)
                {
                    case "100001":
                        BaseRequestParamList sandDFRequestData = new SandDFRequestParamList();
                        initRequestData(sandDFRequestData, this.dgvPayRequest);
                        break;
                    case "100002":
                        BaseRequestParamList sdjDFRequestData = new SdjDFRequestParamList();
                        initRequestData(sdjDFRequestData, this.dgvPayRequest);
                        break;
                    case "100003":
                        BaseRequestParamList yytDFRequestData = new YytDFRequestParamList();
                        initRequestData(yytDFRequestData, this.dgvPayRequest);
                        break;
                }
            }
        }

        private void initRequestData(BaseRequestParamList _requestParamList,DataGridView _toShowDGV) {

            _toShowDGV.DataSource = null;
            _toShowDGV.Rows.Clear();
            foreach (KeyValuePair<string, object> kvp in _requestParamList.GetKeyValue())
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
                            _toShowDGV.Rows.Add(row);
                            break;
                        case "listDic":
                            DataGridViewComboBoxCell cbKeyCell = new DataGridViewComboBoxCell();
                            cbKeyCell.DataSource = (List<DictionaryEntry>)kvp.Value;
                            cbKeyCell.DisplayMember = "Key";
                            cbKeyCell.ValueMember = "Value";
                            row.Cells.Add(cbKeyCell);
                            _toShowDGV.Rows.Add(row);
                            _toShowDGV.Rows[cbKeyCell.RowIndex].Cells[1].Value = ((List<DictionaryEntry>)kvp.Value)[0].Value;
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
            //
            ChannelInfo channelInfo = this.cmbSYS.SelectedItem == null ? null : (ChannelInfo)this.cmbSYS.SelectedItem;
            try
            {
                if (null != channelInfo)
                {
                    Settings cfg = Settings.Default;
                    DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                    string strSelectSql = "select * from ChannelConfig where 1=1 and channelCode='" + channelInfo.SysId + "'";
                    List<MerchantInfo> mchInfoList = dbUtil.QueryForList<MerchantInfo>(strSelectSql, null);
                    if (mchInfoList != null && mchInfoList.Count > 0)
                    {
                        if (channelInfo.SysId.Equals("100003"))
                        {
                            this.tabControl1.SelectedTab = this.tabPage7;
                            this.txtYytMid.Text = mchInfoList[0].MchId;
                            this.txtYytM_name.Text = mchInfoList[0].MchName;
                            this.txtYytT_id.Text = mchInfoList[0].Code2;
                            this.txtYytW_id.Text = mchInfoList[0].Code1;
                            this.txtYytF_Id.Text = mchInfoList[0].Code3;
                            this.txtYytA_id.Text = mchInfoList[0].Code4;
                            this.txtYytA_key.Text = mchInfoList[0].Code5;
                        }
                        else {
                            this.tabControl1.SelectedTab = this.tabPage1;
                            this.txbMchID.Text = mchInfoList[0].MchId;
                            this.txbMchName.Text = mchInfoList[0].MchName;
                            this.txtPFXPath.Text = mchInfoList[0].Code2;
                            this.txtPFXPwd.Text = mchInfoList[0].Code1;
                            this.txtCERPath.Text = mchInfoList[0].Code3;
                            this.txbOrgKey.Text = mchInfoList[0].Code4;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Write("初始化渠道数错误：" + ex.Message);
            }
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
                        BaseRequestParamList sandDKRequestData = new SandDKRequestParamList();
                        initRequestData(sandDKRequestData,this.dgvRequestParam);
                        break;
                    case "100002":
                         BaseRequestParamList sdjDKRequestData = new SdjDKRequestParamList();
                         initRequestData(sdjDKRequestData, this.dgvRequestParam);
                        break;
                    case "100003":
                        BaseRequestParamList yytDKRequestData = new YytDKRequestParamList();
                        initRequestData(yytDKRequestData, this.dgvRequestParam);
                        break;
                }
            }

        }
        /// <summary>
        /// 选择代付商户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPay_SelectedIndexChanged(object sender, EventArgs e)
        {
            MerchantInfo mchInfo = (MerchantInfo)this.cmbPay.SelectedItem;
            if (mchInfo != null)
            {
                switch (mchInfo.ChannelCode)
                {
                    case "100001":
                        BaseRequestParamList sandDFRequestData = new SandDFRequestParamList();
                        initRequestData(sandDFRequestData, this.dgvPayRequest);
                        break;
                    case "100002":
                        BaseRequestParamList sdjDFRequestData = new SdjDFRequestParamList();
                        initRequestData(sdjDFRequestData, this.dgvPayRequest);
                        break;
                    case "100003":
                        BaseRequestParamList yytDFRequestData = new YytDFRequestParamList();
                        initRequestData(yytDFRequestData, this.dgvPayRequest);
                        break;
                }
            }

        }

        #region 单笔（代付，代扣）提交
        /// <summary>
        /// 代付提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaySubmit_Click(object sender, EventArgs e)
        {
            //获取商户信息

            MerchantInfo mchInfo = (MerchantInfo)this.cmbMerchant.SelectedItem;
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
            string orgKey = mchInfo.Code4;
            switch (mchInfo.ChannelCode)
            {
                case "100001":
                    //MessageBox.Show("暂不支持该渠道");
                    doSandDF(this.dgvPayRequest, null, mchId, mchName, pfxPath, pfxPwd, cerPath);
                    break;
                case "100002":
                    doSdjDF(this.dgvPayRequest, null, mchId, mchName, orgKey, pfxPath, pfxPwd, cerPath);
                    break;
                case "100003":
                    MessageBox.Show("暂不支持该渠道");
                    //doYytDF(this.dgvPayRequest, null, mchId, mchName, orgKey, pfxPath, pfxPwd, cerPath);
                    break;

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
            switch (mchInfo.ChannelCode)
            {
                case "100001":
                    doSandDK(this.dgvRequestParam, null, mchInfo.MchId, mchInfo.MchName, mchInfo.Code2, mchInfo.Code1, mchInfo.Code3);
                    break;
                case "100002":
                    doSdjDK(this.dgvRequestParam, null, mchInfo.MchId, mchInfo.MchName, mchInfo.Code4, mchInfo.Code2, mchInfo.Code1, mchInfo.Code3);
                    break;
                case "100003":
                    doYytDK(this.dgvRequestParam, null, mchInfo.MchId, mchInfo.MchName, mchInfo.Code1, mchInfo.Code2, mchInfo.Code3, mchInfo.Code4, mchInfo.Code5);
                    break;

            }

        }
        #endregion

        #region 批量（代扣，代付）提交
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
            string code1 = this.txtPFXPwd.Text;//PFX,JKS私钥文件路径
            string code2 = this.txtPFXPath.Text;//pfx,jks私钥文件密码
            string code3 = this.txtCERPath.Text;//公钥文件路径
            string code4 = this.txbOrgKey.Text;//md5，sha，aes秘钥等
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
                       + ",'"+code4+"'"
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
                    strSelectSql = "select * from ChannelConfig where 1=1";
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


        private void btnYytSave_Click(object sender, EventArgs e)
        {
            string sysId = this.cmbSYS.SelectedValue.ToString();
            string sysName = this.cmbSYS.Text;

            string mchId = this.txtYytMid.Text;
            string mchName = this.txtYytM_name.Text;
            string code1 = this.txtYytW_id.Text;
            string code2 = this.txtYytT_id.Text;
            string code3 = this.txtYytF_Id.Text;
            string code4 = this.txtYytA_id.Text;
            string code5 = this.txtYytA_key.Text;
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
                       + ",'" + code4 + "'"
                       + ",'" + code5 + "'"
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
                    strSelectSql = "select * from ChannelConfig where 1=1";
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
                Thread.Sleep(1000 * 60 * 1);//每1分钟运行一次

                try
                {
                    //获取商户信息
                    string sqlStr = "select * from CollectionOrder c inner join ChannelConfig m on c.mchId=m.mchId  where 1=1 and c.status='0' and c.createTime >=#" + DateTime.Now.AddMinutes(-60) + "# and c.createTime <=#" + DateTime.Now.AddMinutes(-1) + "#";
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
                            string orgKey = dt.Rows[i]["code4"].ToString();
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
                                case "100002":
                                    doSdjQuery(orderId, mchId, transTime, mchName,orgKey, pfxPath, pfxPwd, cerPath, payType, orderType);
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
            List<Dictionary<string, string>> batchDKparams = new List<Dictionary<string, string>>();
            if (dt == null)
            { //单笔
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                int rowCount = RequestParamDGV.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (RequestParamDGV.Rows[i].Cells["DK_VALUE"] == null || RequestParamDGV.Rows[i].Cells["DK_VALUE"].Value == null)
                    {
                        MessageBox.Show("请选择" + RequestParamDGV.Rows[i].Cells["DK_NAME"].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = RequestParamDGV.Rows[i].Cells["DK_NAME"].Value.ToString().Trim();
                    string valueField = RequestParamDGV.Rows[i].Cells["DK_VALUE"].Value.ToString().Trim();

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
                        if (key == "certNo")
                        {
                            tempDic.Add("cardId", value);
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

                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        string isExsitOrderSqlStr = "select * from CollectionOrder c where c.outOrderId='" + dic["orderCode"] + "' and c.mchId='" + mchId + "'";
                        DataTable isExistDt = dbUtil.ExecuteDataTable(isExsitOrderSqlStr, null);
                        if (isExistDt != null && isExistDt.Rows.Count > 0)
                        {
                            MessageBox.Show("该订单号已存在，请更换订单号。");
                            return;
                        }
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
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
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

                    //解密后的服务器返回
                    BaseRequest sandRequest = new SandRequest(dic, mchId, mchName,pfxPath, pfxPwd, cerPath,"");
                    BaseResponse sandResponse=new SandResponse();
                    sandResponse= sandRequest.doCollection();
                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + sandResponse.status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + sandResponse.bankOrderNo + "'"
                               + ",respCode='" + sandResponse.respCode + "'"
                               + ",respMsg='" + sandResponse.respMsg + "'"
                               + " where orderId='" + sandResponse.orderId + "'";
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
                    if ("0000".Equals(sandResponse.returnCode))
                    {
                        if ("1".Equals(sandResponse.status))
                        {
                            successRecord.Add(sandResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣完成，成功！");
                            }
                        }
                        else if ("2".Equals(sandResponse.status))
                        {
                            errorRecord.Add(sandResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣失败，错误原因：" + sandResponse.respMsg);
                            }
                        }
                        else
                        {
                            processingRecord.Add(sandResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(sandResponse.orderId);
                        if (count == batchDKparams.Count)
                        {
                            MessageBox.Show("代扣提交异常！错误码：" + sandResponse.respCode + "(" + sandResponse.respMsg + ")");
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
        private void doSandQuery(string orderId, string mchId, string tranTime, string mchName, string pfxPath, string pfxPwd, string cerPath, string payType, string orderType)
        {
            Settings cfg = Settings.Default;
            DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("version", "01");
            if (payType == "0" && orderType == "dk")
            {
                dic.Add("productId", "00000002");// 代收对公    00000001 代收对私    00000002  代付对私    00000004 
            }
            else if (payType == "1" && orderType == "dk")
            {
                dic.Add("productId", "00000001");// 代收对公    00000001 代收对私    00000002  代付对私    00000004 
            }
            else if (payType == "0" && orderType == "df")
            {
                dic.Add("productId", "00000004");
            }
            else if (payType == "1" && orderType == "df")
            {
                dic.Add("productId", "00000003");
            }
            //DateTime tranDt = DateTime.ParseExact(dic["tranTime"], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

            DateTime datetime;
            if (!DateTime.TryParse(tranTime, out datetime))
            {
                log.Write(orderId + "--时间格式错误，查询失败");
                return;
            }
            dic.Add("tranTime", datetime.ToString("yyyyMMddHHmmss"));
            dic.Add("orderCode", orderId);

            //解密后的服务器返回
            BaseRequest sandRequest = new SandRequest(dic, mchId, mchName, pfxPath, pfxPwd, cerPath, "");
            BaseResponse sandResponse = new SandResponse();
            sandResponse = sandRequest.doQuery();
            try
            {
                string strUpdateSql = "update CollectionOrder set "
                       + " status = " + sandResponse.status
                       + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                       + ", bankOrderNo='" + sandResponse.bankOrderNo + "'"
                       + ",respCode='" + sandResponse.respCode + "'"
                       + ",respMsg='" + sandResponse.respMsg + "'"
                       + " where orderId='" + sandResponse.orderId + "'";
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
        /// 杉德代扣处理
        /// </summary>
        /// <param name="RequestParamDGV"></param>
        /// <param name="dt"></param>
        /// <param name="mchId"></param>
        /// <param name="mchName"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        private void doSandDF(DataGridView RequestParamDGV, DataTable dt, string mchId, string mchName, string pfxPath, string pfxPwd, string cerPath)
        {
            List<Dictionary<string, string>> batchDFparams = new List<Dictionary<string, string>>();
            if (dt == null)
            { //单笔
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                int rowCount = RequestParamDGV.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (RequestParamDGV.Rows[i].Cells[1] == null || RequestParamDGV.Rows[i].Cells[1].Value == null)
                    {
                        MessageBox.Show("请选择" + RequestParamDGV.Rows[i].Cells[0].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = RequestParamDGV.Rows[i].Cells[0].Value.ToString().Trim();
                    string valueField = RequestParamDGV.Rows[i].Cells[1].Value.ToString().Trim();

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
                batchDFparams.Add(tempDic);
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
                        MessageBox.Show("第" + (i + 1) + "行代付金额格式不正确");
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
                    batchDFparams.Add(tempDic);
                }
            }
            try
            {
                int count = 0;
                List<string> successRecord = new List<string>();
                List<string> errorRecord = new List<string>();
                List<string> processingRecord = new List<string>();
                foreach (Dictionary<string, string> dic in batchDFparams)
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
                            dic.Add("productId", "00000004");
                        }
                        else if (dic["accAttr"].Equals("1"))
                        {
                            payType = "1";//对公
                            dic.Add("productId", "00000003");
                        }

                        //杉德的tranTime需要用于查询    
                        DateTime tranDt = DateTime.ParseExact(dic["tranTime"], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        string isExsitOrderSqlStr = "select * from CollectionOrder c where c.outOrderId='" + dic["orderCode"] + "' and c.mchId='" + mchId + "'";
                        DataTable isExistDt = dbUtil.ExecuteDataTable(isExsitOrderSqlStr, null);
                        if (isExistDt != null && isExistDt.Rows.Count > 0)
                        {
                            MessageBox.Show("该订单号已存在，请更换订单号。");
                            return;
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
                               + ",'" + tranDt.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ",null"
                               + ",''"
                               + ",''"
                               + ",''"
                               + ")";
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
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

                    //解密后的服务器返回
                    BaseRequest sandRequest = new SandRequest(dic, mchId, mchName, pfxPath, pfxPwd, cerPath, "");
                    BaseResponse sandResponse = new SandResponse();
                    sandResponse = sandRequest.doAgentPay();
                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + sandResponse.status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + sandResponse.bankOrderNo + "'"
                               + ",respCode='" + sandResponse.respCode + "'"
                               + ",respMsg='" + sandResponse.respMsg + "'"
                               + " where orderId='" + sandResponse.orderId + "'";
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
                    if ("0000".Equals(sandResponse.returnCode))
                    {
                        if ("1".Equals(sandResponse.status))
                        {
                            successRecord.Add(sandResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付完成，成功！");
                            }
                        }
                        else if ("2".Equals(sandResponse.status))
                        {
                            errorRecord.Add(sandResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付失败，错误原因：" + sandResponse.respMsg);
                            }
                        }
                        else
                        {
                            processingRecord.Add(sandResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(sandResponse.orderId);
                        if (count == batchDFparams.Count)
                        {
                            MessageBox.Show("代付提交异常！错误码：" + sandResponse.respCode + "(" + sandResponse.respMsg + ")");
                        }
                    }
                }
                if (1 != batchDFparams.Count)
                {
                    MessageBox.Show("代付完成，共" + batchDFparams.Count + "笔，成功：" + successRecord.Count + "笔，进行中：" + processingRecord.Count + "笔，失败或异常：" + errorRecord.Count + "笔。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Write(ex, MsgType.Error);
            }
        }


        #region TODO:通用方法构建
        private void doCollection(BaseRequest request,DataTable dt,BaseRequestParamList baseRequestParamList)
        {
            /*
            //mchId, mchName, pfxPath, pfxPwd, cerPath, ""
            string mchId = request.mchId;
            string mchName = request.mchName;
            string pfxPath = request.pfxPath;
            string pfxPwd = request.pfxPwd;
            string cerPath = request.cerPath;
            string orgKey = request.orgKey;

            //定义Json转换类
            List<Dictionary<string, string>> batchDKparams = new List<Dictionary<string, string>>();
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
                        foreach (KeyValuePair<string, object> kvp in baseRequestParamList.GetKeyValue())
                        {
                            string[] keySplitStr = kvp.Key.Split('|');
                            if (keySplitStr.Length == 4)//带类型
                            {
                                string enkey = keySplitStr[0];
                                string cnKey = keySplitStr[1];
                                string type = keySplitStr[2];
                                string visble = keySplitStr[3];
                                string value=string.IsNullOrEmpty(dt.Rows[i][cnKey]+"")?kvp.Value.ToString():dt.Rows[i][cnKey].ToString();                                   
                                tempDic.Add(enkey,value);

                            }
                        }
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
                    try{   
                    Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        //保存记录
                        string strInsertSql = "insert into CollectionOrder(orderId,orderType,mchId,mchName,status,amount,payType,bankOrderNo,outOrderId,createTime,updateTime,respCode,respMsg,remark)"
                               + " VALUES ('" + orderId + "'"
                               + ",'dk'"
                               + ",'" + mchId + "'"
                               + ",'" + mchName + "'"
                               + ",0"
                               + "," +0//转为分存储
                               + ",'0'"//对公对私标识(默认对私）
                               + ",''"
                               + ",'" + dic["orderCode"] + "'"
                               + ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ",null"
                               + ",''"
                               + ",''"
                               + ",''"
                               + ")";
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                        //LogUtil.Write("保存成功"+effectNo+"
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        log.Write(ex, MsgType.Error);
                        return;
                    }
                    //解密后的服务器返回
                    BaseRequest sandRequest = new SandRequest(dic, mchId, mchName, pfxPath, pfxPwd, cerPath, "");
                    BaseResponse sandResponse = new SandResponse();
                    sandResponse = sandRequest.doCollection();
                    // MessageCryptWorker.trafficMessage resp = .CollectionMessage(dic, mchId, pfxPath, pfxPwd, cerPath);

                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + sandResponse.status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + sandResponse.bankOrderNo + "'"
                               + ",respCode='" + sandResponse.respCode + "'"
                               + ",respMsg='" + sandResponse.respMsg + "'"
                               + " where orderId='" + sandResponse.orderId + "'";
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


                    if ("0000".Equals(sandResponse.returnCode))
                    {
                        if ("1".Equals(sandResponse.status))
                        {
                            successRecord.Add(sandResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣完成，成功！");
                            }
                        }
                        else if ("2".Equals(sandResponse.status))
                        {
                            errorRecord.Add(sandResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣失败，错误原因：" + sandResponse.respMsg);
                            }
                        }
                        else
                        {
                            processingRecord.Add(sandResponse.status);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(sandResponse.status);
                        if (count == batchDKparams.Count)
                        {
                            MessageBox.Show("代扣提交异常！错误码：" + sandResponse.respCode + "(" + sandResponse.respMsg + ")");
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
        */
        }
#endregion
        #endregion

        #region 盛迪嘉支付平台


        /// <summary>
        /// 盛迪嘉代扣处理
        /// </summary>
        /// <param name="RequestParamDGV"></param>
        /// <param name="dt"></param>
        /// <param name="mchId"></param>
        /// <param name="mchName"></param>
        /// <param name="OrgKey"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        private void doSdjDK(DataGridView RequestParamDGV, DataTable dt, string mchId, string mchName, string orgKey,string pfxPath, string pfxPwd, string cerPath)
        {
            List<Dictionary<string, string>> batchDKparams = new List<Dictionary<string, string>>();
            if (dt == null)
            { //单笔
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                int rowCount = RequestParamDGV.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (RequestParamDGV.Rows[i].Cells["DK_VALUE"] == null || RequestParamDGV.Rows[i].Cells["DK_VALUE"].Value == null)
                    {
                        MessageBox.Show("请选择" + RequestParamDGV.Rows[i].Cells["DK_NAME"].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = RequestParamDGV.Rows[i].Cells["DK_NAME"].Value.ToString().Trim();
                    string valueField = RequestParamDGV.Rows[i].Cells["DK_VALUE"].Value.ToString().Trim();

                    if (nameField != null && nameField.Split('|').Length == 2)
                    {
                        key = nameField.Split('|')[0];
                        value = valueField;
                        if (key == "payAmount")
                        {
                            int amount = 0;
                            if (!int.TryParse(value, out amount))
                            {
                                MessageBox.Show("代扣金额格式不正确");
                                return;
                            }
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
                    string amountStr = (Convert.ToInt64(amount) * 100).ToString();
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
                        string payType = "0";//0:对私,1:对公
                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        string isExsitOrderSqlStr = "select * from CollectionOrder c where c.outOrderId='" + dic["orderNo"] + "' and c.mchId='" + mchId + "'";
                        DataTable isExistDt = dbUtil.ExecuteDataTable(isExsitOrderSqlStr, null);
                        if (isExistDt != null && isExistDt.Rows.Count > 0)
                        {
                            MessageBox.Show("该订单号已存在，请更换订单号。");
                            return;
                        }
                        //保存记录
                        string strInsertSql = "insert into CollectionOrder(orderId,orderType,mchId,mchName,status,amount,payType,bankOrderNo,outOrderId,createTime,updateTime,respCode,respMsg,remark)"
                               + " VALUES ('" + orderId + "'"
                               + ",'dk'"
                               + ",'" + mchId + "'"
                               + ",'" + mchName + "'"
                               + ",0"
                               + "," + Convert.ToInt64(dic["payAmount"])*100//转为分存储
                               + ",'" + payType + "'"//对公对私标识
                               + ",''"
                               + ",'" + dic["orderNo"] + "'"
                               + ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ",null"
                               + ",''"
                               + ",''"
                               + ",''"
                               + ")";
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        log.Write(ex, MsgType.Error);
                        return;
                    }
                    //将订单号变为数据库数据，防止订单重复。
                    dic.Remove("orderNo");
                    dic.Add("orderNo", orderId);
                    BaseRequest sdjRequest = new SdjRequest(dic, mchId, mchName, pfxPath, pfxPwd, cerPath, orgKey);
                    BaseResponse sdjResponse = new SdjResponse();
                    sdjResponse = sdjRequest.doCollection();
                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + sdjResponse.status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + sdjResponse.bankOrderNo + "'"
                               + ",respCode='" + sdjResponse.respCode + "'"
                               + ",respMsg='" + sdjResponse.respMsg + "'"
                               + " where orderId='" + sdjResponse.orderId + "'";
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
                    if ("0000".Equals(sdjResponse.returnCode))
                    {
                        if ("1".Equals(sdjResponse.status))
                        {
                            successRecord.Add(sdjResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣完成，成功！");
                            }
                        }
                        else if ("2".Equals(sdjResponse.status))
                        {
                            errorRecord.Add(sdjResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣失败，错误原因：" + sdjResponse.respMsg);
                            }
                        }
                        else
                        {
                            processingRecord.Add(sdjResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(sdjResponse.orderId);
                        if (count == batchDKparams.Count)
                        {
                            MessageBox.Show("代扣提交异常！错误码：" + sdjResponse.respCode + "(" + sdjResponse.respMsg + ")");
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
        /// 盛迪嘉查询补单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="mchId"></param>
        /// <param name="mchName"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        /// <param name="payType"></param>
        private void doSdjQuery(string orderId, string mchId, string tranTime, string mchName,string orgKey, string pfxPath, string pfxPwd, string cerPath, string payType, string orderType)
        {
            Settings cfg = Settings.Default;
            DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("orderNo", orderId);
            //解密后的服务器返回
            BaseRequest sdjRequest = new SdjRequest(dic, mchId, mchName, pfxPath, pfxPwd, cerPath, orgKey);
            BaseResponse sdjResponse = new SdjResponse();
            sdjResponse = sdjRequest.doQuery();
            try
            {
                string strUpdateSql = "update CollectionOrder set "
                       + " status = " + sdjResponse.status
                       + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                       + ", bankOrderNo='" + sdjResponse.bankOrderNo + "'"
                       + ",respCode='" + sdjResponse.respCode + "'"
                       + ",respMsg='" + sdjResponse.respMsg + "'"
                       + " where orderId='" + sdjResponse.orderId + "'";
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
        /// 盛迪嘉代付处理
        /// </summary>
        /// <param name="RequestParamDGV"></param>
        /// <param name="dt"></param>
        /// <param name="mchId"></param>
        /// <param name="mchName"></param>
        /// <param name="OrgKey"></param>
        /// <param name="pfxPath"></param>
        /// <param name="pfxPwd"></param>
        /// <param name="cerPath"></param>
        private void doSdjDF(DataGridView RequestParamDGV, DataTable dt, string mchId, string mchName, string orgKey, string pfxPath, string pfxPwd, string cerPath)
        {
            List<Dictionary<string, string>> batchDFparams = new List<Dictionary<string, string>>();
            if (dt == null)
            { //单笔
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                int rowCount = RequestParamDGV.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (RequestParamDGV.Rows[i].Cells[1] == null || RequestParamDGV.Rows[i].Cells[1].Value == null)
                    {
                        MessageBox.Show("请选择" + RequestParamDGV.Rows[i].Cells[0].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = RequestParamDGV.Rows[i].Cells[0].Value.ToString().Trim();
                    string valueField = RequestParamDGV.Rows[i].Cells[1].Value.ToString().Trim();
                    string textField = RequestParamDGV.Rows[i].Cells[1].Value.ToString().Trim();
                    DataGridViewCell cell = RequestParamDGV.Rows[i].Cells[1];//得到单元格
                    if (cell is DataGridViewComboBoxCell)
                    {
                        DataGridViewComboBoxCell cb = cell as DataGridViewComboBoxCell;

                        textField = cb.EditedFormattedValue.ToString();
                    }
                    if (nameField != null && nameField.Split('|').Length == 2)
                    {
                        key = nameField.Split('|')[0];
                        value = valueField;
                        if (key == "payAmount")
                        {
                            int amount = 0;
                            if (!int.TryParse(value, out amount))
                            {
                                MessageBox.Show("代付金额格式不正确");
                                return;
                            }
                        }
                        if (key == "payeeBankName")//银行和Code特殊处理
                        {
                            if (tempDic.ContainsKey("bankCode")) {
                                tempDic.Remove("bankCode");
                                tempDic.Add("bankCode", value);    
                            }                          
                            tempDic.Add(key, textField);
                            continue;
                        }
                        tempDic.Add(key, value);
                    }
                }
                batchDFparams.Add(tempDic);
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
                    batchDFparams.Add(tempDic);
                }
            }
            try
            {
                int count = 0;
                List<string> successRecord = new List<string>();
                List<string> errorRecord = new List<string>();
                List<string> processingRecord = new List<string>();
                foreach (Dictionary<string, string> dic in batchDFparams)
                {
                    count++;
                    string orderId = "200" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(2) + ComUtils.CreateRandomNum(4);
                    //orderId,mchId,mchName,status,amount,bankOrderNo,outOrderId,,createTime,updateTime,respCode,respMsg,remark
                    try
                    {
                        string payType = "0";//0:对私,1:对公
                        if (dic["bankAccountType"].Equals("INDIVIDUAL"))
                        {
                            payType = "0";//对私
                        }
                        else if (dic["bankAccountType"].Equals("OPEN"))
                        {
                            payType = "1";//对公
                        }
                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        string isExsitOrderSqlStr = "select * from CollectionOrder c where c.outOrderId='" + dic["orderNo"] + "' and c.mchId='" + mchId + "'";
                        DataTable isExistDt = dbUtil.ExecuteDataTable(isExsitOrderSqlStr, null);
                        if (isExistDt != null && isExistDt.Rows.Count > 0)
                        {
                            MessageBox.Show("该订单号已存在，请更换订单号。");
                            return;
                        }
                        //保存记录
                        string strInsertSql = "insert into CollectionOrder(orderId,orderType,mchId,mchName,status,amount,payType,bankOrderNo,outOrderId,createTime,updateTime,respCode,respMsg,remark)"
                               + " VALUES ('" + orderId + "'"
                               + ",'df'"
                               + ",'" + mchId + "'"
                               + ",'" + mchName + "'"
                               + ",0"
                               + "," + Convert.ToInt64(dic["payAmount"])*100//转为分存储
                               + ",'" + payType + "'"//对公对私标识
                               + ",''"
                               + ",'" + dic["orderNo"] + "'"
                               + ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ",null"
                               + ",''"
                               + ",''"
                               + ",''"
                               + ")";
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        log.Write(ex, MsgType.Error);
                        return;
                    }
                    //将订单号变为数据库数据，防止订单重复。
                    dic.Remove("orderNo");
                    dic.Add("orderNo", orderId);
                    BaseRequest sdjRequest = new SdjRequest(dic, mchId, mchName, pfxPath, pfxPwd, cerPath, orgKey);
                    BaseResponse sdjResponse = new SdjResponse();
                    sdjResponse = sdjRequest.doAgentPay();
                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + sdjResponse.status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + sdjResponse.bankOrderNo + "'"
                               + ",respCode='" + sdjResponse.respCode + "'"
                               + ",respMsg='" + sdjResponse.respMsg + "'"
                               + " where orderId='" + sdjResponse.orderId + "'";
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
                    if ("0000".Equals(sdjResponse.returnCode))
                    {
                        if ("1".Equals(sdjResponse.status))
                        {
                            successRecord.Add(sdjResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付完成，成功！");
                            }
                        }
                        else if ("2".Equals(sdjResponse.status))
                        {
                            errorRecord.Add(sdjResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付失败，错误原因：" + sdjResponse.respMsg);
                            }
                        }
                        else
                        {
                            processingRecord.Add(sdjResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(sdjResponse.orderId);
                        if (count == batchDFparams.Count)
                        {
                            MessageBox.Show("代付提交异常！错误码：" + sdjResponse.respCode + "(" + sdjResponse.respMsg + ")");
                        }
                    }
                }
                if (1 != batchDFparams.Count)
                {
                    MessageBox.Show("代付完成，共" + batchDFparams.Count + "笔，成功：" + successRecord.Count + "笔，进行中：" + processingRecord.Count + "笔，失败或异常：" + errorRecord.Count + "笔。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Write(ex, MsgType.Error);
            }
        }

        #endregion

        #region 银盈通平台
        private void doYytDK(DataGridView RequestParamDGV, DataTable dt, string mchId, string mchName, string tId, string wId, string fId, string aId,string appKey)
        {
            List<Dictionary<string, string>> batchDKparams = new List<Dictionary<string, string>>();
            if (dt == null)
            { //单笔
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                int rowCount = RequestParamDGV.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (RequestParamDGV.Rows[i].Cells["DK_VALUE"] == null || RequestParamDGV.Rows[i].Cells["DK_VALUE"].Value == null)
                    {
                        MessageBox.Show("请选择" + RequestParamDGV.Rows[i].Cells["DK_NAME"].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = RequestParamDGV.Rows[i].Cells["DK_NAME"].Value.ToString().Trim();
                    string valueField = RequestParamDGV.Rows[i].Cells["DK_VALUE"].Value.ToString().Trim();

                    if (nameField != null && nameField.Split('|').Length == 2)
                    {
                        key = nameField.Split('|')[0];
                        value = valueField;
                        if (key == "amount")
                        {
                            int amount = 0;
                            if (!int.TryParse(value, out amount))
                            {
                                MessageBox.Show("代扣金额格式不正确");
                                return;
                            }
                        }
                        if (key == "province_code")
                        {
                            value = value.PadRight(6, '0');   
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
                    string amountStr = (Convert.ToInt64(amount) * 100).ToString();
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
                        string payType = "0";//0:对私,1:对公
                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        string isExsitOrderSqlStr = "select * from CollectionOrder c where c.outOrderId='" + dic["order_number"] + "' and c.mchId='" + mchId + "'";
                        DataTable isExistDt = dbUtil.ExecuteDataTable(isExsitOrderSqlStr, null);
                        if (isExistDt != null && isExistDt.Rows.Count > 0)
                        {
                            MessageBox.Show("该订单号已存在，请更换订单号。");
                            return;
                        }
                        //保存记录
                        string strInsertSql = "insert into CollectionOrder(orderId,orderType,mchId,mchName,status,amount,payType,bankOrderNo,outOrderId,createTime,updateTime,respCode,respMsg,remark)"
                               + " VALUES ('" + orderId + "'"
                               + ",'dk'"
                               + ",'" + mchId + "'"
                               + ",'" + mchName + "'"
                               + ",0"
                               + "," + Convert.ToInt64(dic["amount"]) * 100//转为分存储
                               + ",'" + payType + "'"//对公对私标识
                               + ",''"
                               + ",'" + dic["order_number"] + "'"
                               + ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ",null"
                               + ",''"
                               + ",''"
                               + ",''"
                               + ")";
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        log.Write(ex, MsgType.Error);
                        return;
                    }
                    //将订单号变为数据库数据，防止订单重复。
                    dic.Remove("order_number");
                    dic.Add("order_number", orderId);
                    dic.Add("req_no", orderId);
                    BaseRequest  yytRequest = new YytRequest(dic, mchId, mchName, tId, wId, fId, aId,appKey);
                    BaseResponse yytResponse = new YytResponse();
                    yytResponse = yytRequest.doCollection();
                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + yytResponse.status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + yytResponse.bankOrderNo + "'"
                               + ",respCode='" + yytResponse.respCode + "'"
                               + ",respMsg='" + yytResponse.respMsg + "'"
                               + " where orderId='" + yytResponse.orderId + "'";
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
                    if ("0000".Equals(yytResponse.returnCode))
                    {
                        if ("1".Equals(yytResponse.status))
                        {
                            successRecord.Add(yytResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣完成，成功！");
                            }
                        }
                        else if ("2".Equals(yytResponse.status))
                        {
                            errorRecord.Add(yytResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣失败，错误原因：" + yytResponse.respMsg);
                            }
                        }
                        else
                        {
                            processingRecord.Add(yytResponse.orderId);
                            if (1 == batchDKparams.Count)
                            {
                                MessageBox.Show("代扣提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(yytResponse.orderId);
                        if (count == batchDKparams.Count)
                        {
                            MessageBox.Show("代扣提交异常！错误码：" + yytResponse.respCode + "(" + yytResponse.respMsg + ")");
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

        private void doYytDF(DataGridView RequestParamDGV, DataTable dt, string mchId, string mchName, string orgKey, string pfxPath, string pfxPwd, string cerPath)
        {
            List<Dictionary<string, string>> batchDFparams = new List<Dictionary<string, string>>();
            if (dt == null)
            { //单笔
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                int rowCount = RequestParamDGV.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    string key = "";
                    string value = "";
                    if (RequestParamDGV.Rows[i].Cells[1] == null || RequestParamDGV.Rows[i].Cells[1].Value == null)
                    {
                        MessageBox.Show("请选择" + RequestParamDGV.Rows[i].Cells[0].Value.ToString().Trim() + "的值");
                        return;
                    }
                    string nameField = RequestParamDGV.Rows[i].Cells[0].Value.ToString().Trim();
                    string valueField = RequestParamDGV.Rows[i].Cells[1].Value.ToString().Trim();
                    string textField = RequestParamDGV.Rows[i].Cells[1].Value.ToString().Trim();
                    DataGridViewCell cell = RequestParamDGV.Rows[i].Cells[1];//得到单元格
                    if (cell is DataGridViewComboBoxCell)
                    {
                        DataGridViewComboBoxCell cb = cell as DataGridViewComboBoxCell;

                        textField = cb.EditedFormattedValue.ToString();
                    }
                    if (nameField != null && nameField.Split('|').Length == 2)
                    {
                        key = nameField.Split('|')[0];
                        value = valueField;
                        if (key == "payAmount")
                        {
                            int amount = 0;
                            if (!int.TryParse(value, out amount))
                            {
                                MessageBox.Show("代付金额格式不正确");
                                return;
                            }
                        }
                        if (key == "payeeBankName")//银行和Code特殊处理
                        {
                            if (tempDic.ContainsKey("bankCode"))
                            {
                                tempDic.Remove("bankCode");
                                tempDic.Add("bankCode", value);
                            }
                            tempDic.Add(key, textField);
                            continue;
                        }
                        tempDic.Add(key, value);
                    }
                }
                batchDFparams.Add(tempDic);
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
                    batchDFparams.Add(tempDic);
                }
            }
            try
            {
                int count = 0;
                List<string> successRecord = new List<string>();
                List<string> errorRecord = new List<string>();
                List<string> processingRecord = new List<string>();
                foreach (Dictionary<string, string> dic in batchDFparams)
                {
                    count++;
                    string orderId = "200" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(2) + ComUtils.CreateRandomNum(4);
                    //orderId,mchId,mchName,status,amount,bankOrderNo,outOrderId,,createTime,updateTime,respCode,respMsg,remark
                    try
                    {
                        string payType = "0";//0:对私,1:对公
                        if (dic["bankAccountType"].Equals("INDIVIDUAL"))
                        {
                            payType = "0";//对私
                        }
                        else if (dic["bankAccountType"].Equals("OPEN"))
                        {
                            payType = "1";//对公
                        }
                        Settings cfg = Settings.Default;
                        DbUtility dbUtil = new DbUtility(cfg.localConnectionString, DbProviderType.OleDb);
                        string isExsitOrderSqlStr = "select * from CollectionOrder c where c.outOrderId='" + dic["orderNo"] + "' and c.mchId='" + mchId + "'";
                        DataTable isExistDt = dbUtil.ExecuteDataTable(isExsitOrderSqlStr, null);
                        if (isExistDt != null && isExistDt.Rows.Count > 0)
                        {
                            MessageBox.Show("该订单号已存在，请更换订单号。");
                            return;
                        }
                        //保存记录
                        string strInsertSql = "insert into CollectionOrder(orderId,orderType,mchId,mchName,status,amount,payType,bankOrderNo,outOrderId,createTime,updateTime,respCode,respMsg,remark)"
                               + " VALUES ('" + orderId + "'"
                               + ",'df'"
                               + ",'" + mchId + "'"
                               + ",'" + mchName + "'"
                               + ",0"
                               + "," + Convert.ToInt64(dic["payAmount"]) * 100//转为分存储
                               + ",'" + payType + "'"//对公对私标识
                               + ",''"
                               + ",'" + dic["orderNo"] + "'"
                               + ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ",null"
                               + ",''"
                               + ",''"
                               + ",''"
                               + ")";
                        int effectNo = dbUtil.ExecuteNonQuery(strInsertSql, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        log.Write(ex, MsgType.Error);
                        return;
                    }
                    //将订单号变为数据库数据，防止订单重复。
                    dic.Remove("orderNo");
                    dic.Add("orderNo", orderId);
                    BaseRequest sdjRequest = new SdjRequest(dic, mchId, mchName, pfxPath, pfxPwd, cerPath, orgKey);
                    BaseResponse sdjResponse = new SdjResponse();
                    sdjResponse = sdjRequest.doAgentPay();
                    try
                    {
                        string strInsertSql = "update CollectionOrder set "
                               + " status = " + sdjResponse.status
                               + ", updateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'"
                               + ", bankOrderNo='" + sdjResponse.bankOrderNo + "'"
                               + ",respCode='" + sdjResponse.respCode + "'"
                               + ",respMsg='" + sdjResponse.respMsg + "'"
                               + " where orderId='" + sdjResponse.orderId + "'";
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
                    if ("0000".Equals(sdjResponse.returnCode))
                    {
                        if ("1".Equals(sdjResponse.status))
                        {
                            successRecord.Add(sdjResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付完成，成功！");
                            }
                        }
                        else if ("2".Equals(sdjResponse.status))
                        {
                            errorRecord.Add(sdjResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付失败，错误原因：" + sdjResponse.respMsg);
                            }
                        }
                        else
                        {
                            processingRecord.Add(sdjResponse.orderId);
                            if (1 == batchDFparams.Count)
                            {
                                MessageBox.Show("代付提交完成，银行处理中");
                            }
                        }
                    }
                    else
                    {
                        errorRecord.Add(sdjResponse.orderId);
                        if (count == batchDFparams.Count)
                        {
                            MessageBox.Show("代付提交异常！错误码：" + sdjResponse.respCode + "(" + sdjResponse.respMsg + ")");
                        }
                    }
                }
                if (1 != batchDFparams.Count)
                {
                    MessageBox.Show("代付完成，共" + batchDFparams.Count + "笔，成功：" + successRecord.Count + "笔，进行中：" + processingRecord.Count + "笔，失败或异常：" + errorRecord.Count + "笔。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Write(ex, MsgType.Error);
            }
        }

        #endregion

    }
}
