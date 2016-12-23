using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
#pragma warning disable 1591

namespace XYZZ.Tools
{
    /// <summary>
    /// Email操作类
    /// </summary>
    public class Email
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public enum ServerHost
        {
            [Addtion(Host = "smtp.sina.com.cn", Port = 25)]
            Sina,
            [Addtion(Host = "smtp.vip.sina.com", Port = 25)]
            SinaVIP,
            [Addtion(Host = "smtp.126.com", Port = 25)]
            网易126邮箱,
            [Addtion(Host = "smtp.139.com", Port = 25)]
            网易139邮箱,
            [Addtion(Host = "smtp.163.com", Port = 25)]
            网易163邮箱,
            [Addtion(Host = "smtp.qq.com", Port = 25)]
            QQ邮箱,
            [Addtion(Host = "smtp.exmail.qq.com", Port = 587)]
            QQ企业邮箱,
            [Addtion(Host = "smtp.mail.yahoo.com.cn", Port = 587)]
            Yahoo中国,
        }

        private MailMessage mMailMessage;  //主要处理发送邮件的内容（如：收发人地址、标题、主体、图片等等）
        private ServerHost mServerHost;  //发件箱的邮件服务器信息
        private string mSenderPassword;  //发件箱的密码
        private string mSenderUsername;  //发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）

        ///<summary>
        /// 构造函数
        ///</summary>
        ///<param name="serverHost">发件箱的邮件服务器地址</param>
        ///<param name="toMail">收件人地址</param>
        ///<param name="fromMail">发件人地址</param>
        ///<param name="username">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）</param>
        ///<param name="password">发件人邮箱密码</param>
        public Email(ServerHost serverHost, List<string> toMail, string fromMail, string username, string password)
        {
            try
            {
                mMailMessage = new MailMessage();
                foreach(string mail in toMail)
                {
                    mMailMessage.To.Add(mail);
                }
                mMailMessage.From = new MailAddress(fromMail);
                mMailMessage.Priority = MailPriority.Normal;
                mSenderUsername = username;
                mSenderPassword = password;
                mServerHost = serverHost;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ///<summary>
        /// 添加附件
        ///</summary>
        ///<param name="attachmentsPath">附件的路径集合，以分号分隔</param>
        public void AddAttachments(string attachmentsPath)
        {
            try
            {
                foreach (string path in attachmentsPath.Split(';'))
                {
                    Attachment data = new Attachment(path, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(path);
                    disposition.ModificationDate = File.GetLastWriteTime(path);
                    disposition.ReadDate = File.GetLastAccessTime(path);
                    mMailMessage.Attachments.Add(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ///<summary>
        /// 发送邮件
        ///</summary>
        ///<param name="subject">邮件标题</param>
        ///<param name="emailBody">邮件内容（以html格式进行设计）</param>
        ///<param name="sslEnable">是否对邮件内容进行socket层加密传输</param>
        ///<param name="pwdCheckEnable">是否对发件人邮箱进行密码验证</param>
        public void Send(string subject, string emailBody, bool sslEnable = true, bool pwdCheckEnable = false)
        {
            try
            {
                if (mMailMessage != null)
                {
                    mMailMessage.Subject = subject;
                    mMailMessage.Body = emailBody;
                    mMailMessage.IsBodyHtml = true;
                    mMailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    AddtionAttribute attribute = (AddtionAttribute)typeof(ServerHost).GetField(mServerHost.ToString()).GetCustomAttributes(false)[0];
                    SmtpClient mSmtpClient = new SmtpClient();
                    mSmtpClient.Host = attribute.Host;
                    mSmtpClient.Port = attribute.Port;
                    mSmtpClient.UseDefaultCredentials = false;
                    mSmtpClient.EnableSsl = sslEnable;
                    if (pwdCheckEnable)
                    {
                        System.Net.NetworkCredential nc = new System.Net.NetworkCredential(mSenderUsername, mSenderPassword);
                        mSmtpClient.Credentials = nc.GetCredential(mSmtpClient.Host, mSmtpClient.Port, "NTLM");
                    }
                    else
                    {
                        mSmtpClient.Credentials = new System.Net.NetworkCredential(mSenderUsername, mSenderPassword);
                    }
                    mSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mSmtpClient.Send(mMailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public class AddtionAttribute : Attribute
        {
            /// <summary>
            /// 地址
            /// </summary>
            public string Host { get; set; }
            /// <summary>
            /// 端口号
            /// </summary>
            public int Port { get; set; }
        }
    }
}
