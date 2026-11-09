using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tea;

namespace RefreshCdn.Client
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 界面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            // 先读取参数
            txtAccessKeyId.Text = Properties.Settings.Default.AccessKeyId;
            txtAccessKeySecret.Text = Properties.Settings.Default.AccessKeySecret;
            txtUrl.Text = Properties.Settings.Default.Url;
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAccessKeyId.Text) || string.IsNullOrEmpty(txtAccessKeySecret.Text))
            {
                MessageBox.Show("请输入 AccessKeyId 和 AccessKeySecret");
                return;
            }
            else if (string.IsNullOrEmpty(txtUrl.Text))
            {
                MessageBox.Show("请输入 URL");
                return;
            }
            Properties.Settings.Default.AccessKeyId = txtAccessKeyId.Text;
            Properties.Settings.Default.AccessKeySecret = txtAccessKeySecret.Text;
            Properties.Settings.Default.Url = txtUrl.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("保存成功");
        }

        /// <summary>
        /// 预热URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForceRefresh_Click(object sender, EventArgs e)
        {
            if (!CheckParams())
            {
                MessageBox.Show("请先保存参数");
                return;
            }
            // 使用AK初始化Credentials Client。
            Aliyun.Credentials.Models.Config credentialsConfig = new Aliyun.Credentials.Models.Config()
            {
                // 凭证类型。
                Type = "access_key",
                // 从环境变量中获取AccessKey ID值。
                AccessKeyId = Properties.Settings.Default.AccessKeyId,
                // 从环境变量中获取AccessKey Secret值。
                AccessKeySecret = Properties.Settings.Default.AccessKeySecret
            };
            Aliyun.Credentials.Client credentialClient = new Aliyun.Credentials.Client(credentialsConfig);
            AlibabaCloud.OpenApiClient.Models.Config cdnConfig = new AlibabaCloud.OpenApiClient.Models.Config();
            // 配置云产品服务接入地址（endpoint）。
            cdnConfig.Endpoint = "cdn.aliyuncs.com";
            // 使用Credentials配置凭证。
            cdnConfig.Credential = credentialClient;
            // 初始化ECS Client。
            AlibabaCloud.SDK.Cdn20180510.Client cdnClient = new AlibabaCloud.SDK.Cdn20180510.Client(cdnConfig);
            AlibabaCloud.SDK.Cdn20180510.Models.PushObjectCacheRequest pushObjectCacheRequest = new AlibabaCloud.SDK.Cdn20180510.Models.PushObjectCacheRequest
            {
                ObjectPath = Properties.Settings.Default.Url,
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                AlibabaCloud.SDK.Cdn20180510.Models.PushObjectCacheResponse resp = cdnClient.PushObjectCacheWithOptions(pushObjectCacheRequest, runtime);
                MessageBox.Show(JsonConvert.SerializeObject(resp, Formatting.Indented));
            }
            catch (TeaException error)
            {
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                MessageBox.Show(error.Message);
                // 诊断地址
                MessageBox.Show(error.Data["Recommend"].ToString());
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                MessageBox.Show(error.Message);
                // 诊断地址
                MessageBox.Show(error.Data["Recommend"].ToString());
            }
        }

        /// <summary>
        /// 检查参数
        /// </summary>
        private bool CheckParams()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.AccessKeyId) ||
                string.IsNullOrEmpty(Properties.Settings.Default.AccessKeySecret) ||
                string.IsNullOrEmpty(Properties.Settings.Default.Url))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshCache_Click(object sender, EventArgs e)
        {
            if (!CheckParams())
            {
                MessageBox.Show("请先保存参数");
                return;
            }
            // 使用AK初始化Credentials Client。
            Aliyun.Credentials.Models.Config credentialsConfig = new Aliyun.Credentials.Models.Config()
            {
                // 凭证类型。
                Type = "access_key",
                // 从环境变量中获取AccessKey ID值。
                AccessKeyId = Properties.Settings.Default.AccessKeyId,
                // 从环境变量中获取AccessKey Secret值。
                AccessKeySecret = Properties.Settings.Default.AccessKeySecret
            };
            Aliyun.Credentials.Client credentialClient = new Aliyun.Credentials.Client(credentialsConfig);
            AlibabaCloud.OpenApiClient.Models.Config cdnConfig = new AlibabaCloud.OpenApiClient.Models.Config();
            // 配置云产品服务接入地址（endpoint）。
            cdnConfig.Endpoint = "cdn.aliyuncs.com";
            // 使用Credentials配置凭证。
            cdnConfig.Credential = credentialClient;
            // 初始化ECS Client。
            AlibabaCloud.SDK.Cdn20180510.Client cdnClient = new AlibabaCloud.SDK.Cdn20180510.Client(cdnConfig);
            AlibabaCloud.SDK.Cdn20180510.Models.RefreshObjectCachesRequest refreshObjectCachesRequest = new AlibabaCloud.SDK.Cdn20180510.Models.RefreshObjectCachesRequest
            {
                ObjectPath = Properties.Settings.Default.Url,
                ObjectType = "Directory",
                Force = true,
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                AlibabaCloud.SDK.Cdn20180510.Models.RefreshObjectCachesResponse resp = cdnClient.RefreshObjectCachesWithOptions(refreshObjectCachesRequest, runtime);
                MessageBox.Show(JsonConvert.SerializeObject(resp, Formatting.Indented));
            }
            catch (TeaException error)
            {
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                MessageBox.Show(error.Message);
                // 诊断地址
                MessageBox.Show(error.Data["Recommend"].ToString());
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                MessageBox.Show(error.Message);
                // 诊断地址
                MessageBox.Show(error.Data["Recommend"].ToString());
            }
        }
    }
}
