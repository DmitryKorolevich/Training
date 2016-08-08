using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SendGrid.Helpers.Mail
{
    /// <summary>
    ///     Class Mail builds an object that sends an email through SendGrid.
    /// </summary>
    public class Mail
    {
        private static readonly JsonSerializer Serializer;

        static Mail()
        {
            Serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                StringEscapeHandling = StringEscapeHandling.Default
            });
        }

        public Mail(Email from, string subject, Email to, Content content)
        {
            From = from;
            Personalization personalization = new Personalization();
            personalization.AddTo(to);
            AddPersonalization(personalization);
            Subject = subject;
            AddContent(content);
        }

        [JsonProperty(PropertyName = "from")]
        public Email From { get; set; }

        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "personalizations")]
        public List<Personalization> Personalization { get; set; }

        [JsonProperty(PropertyName = "content")]
        public List<Content> Contents { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty(PropertyName = "template_id")]
        public string TemplateId { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty(PropertyName = "sections")]
        public Dictionary<string, string> Sections { get; set; }

        [JsonProperty(PropertyName = "categories")]
        public List<string> Categories { get; set; }

        [JsonProperty(PropertyName = "custom_args")]
        public Dictionary<string, string> CustomArgs { get; set; }

        [JsonProperty(PropertyName = "send_at")]
        public long? SendAt { get; set; }

        [JsonProperty(PropertyName = "asm")]
        public Asm Asm { get; set; }

        [JsonProperty(PropertyName = "batch_id")]
        public string BatchId { get; set; }

        [JsonProperty(PropertyName = "ip_pool_name")]
        public string SetIpPoolId { get; set; }

        [JsonProperty(PropertyName = "mail_settings")]
        public MailSettings MailSettings { get; set; }

        [JsonProperty(PropertyName = "tracking_settings")]
        public TrackingSettings TrackingSettings { get; set; }

        [JsonProperty(PropertyName = "reply_to")]
        public Email ReplyTo { get; set; }

        public void AddPersonalization(Personalization personalization)
        {
            if (Personalization == null)
            {
                Personalization = new List<Personalization>();
            }
            Personalization.Add(personalization);
        }

        public void AddContent(Content content)
        {
            if (Contents == null)
            {
                Contents = new List<Content>();
            }
            Contents.Add(content);
        }

        public void AddAttachment(Attachment attachment)
        {
            if (Attachments == null)
            {
                Attachments = new List<Attachment>();
            }
            Attachments.Add(attachment);
        }

        public void AddHeader(string key, string value)
        {
            if (Headers == null)
            {
                Headers = new Dictionary<string, string>();
            }
            Headers.Add(key, value);
        }

        public void AddSection(string key, string value)
        {
            if (Sections == null)
            {
                Sections = new Dictionary<string, string>();
            }
            Sections.Add(key, value);
        }

        public void AddCategory(string category)
        {
            if (Categories == null)
            {
                Categories = new List<string>();
            }
            Categories.Add(category);
        }

        public void AddCustomArgs(string key, string value)
        {
            if (CustomArgs == null)
            {
                CustomArgs = new Dictionary<string, string>();
            }
            CustomArgs.Add(key, value);
        }

        public string GetJsonString()
        {
            using (var stringWriter = new StringWriter(new StringBuilder((Contents?.Sum(c => c.Value.Length) ?? 0)*2)))
            {
                Serializer.Serialize(stringWriter, this);
                return stringWriter.ToString();
            }
        }
    }


    public class ClickTracking
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        [JsonProperty(PropertyName = "enable_text")]
        public bool? EnableText { get; set; }
    }


    public class OpenTracking
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        [JsonProperty(PropertyName = "substitution_tag")]
        public string SubstitutionTag { get; set; }
    }


    public class SubscriptionTracking
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "html")]
        public string Html { get; set; }

        [JsonProperty(PropertyName = "substitution_tag")]
        public string SubstitutionTag { get; set; }
    }


    public class Ganalytics
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        [JsonProperty(PropertyName = "utm_source")]
        public string UtmSource { get; set; }

        [JsonProperty(PropertyName = "utm_medium")]
        public string UtmMedium { get; set; }

        [JsonProperty(PropertyName = "utm_term")]
        public string UtmTerm { get; set; }

        [JsonProperty(PropertyName = "utm_content")]
        public string UtmContent { get; set; }

        [JsonProperty(PropertyName = "utm_campaign")]
        public string UtmCampaign { get; set; }
    }


    public class TrackingSettings
    {
        [JsonProperty(PropertyName = "click_tracking")]
        public ClickTracking ClickTracking { get; set; }

        [JsonProperty(PropertyName = "open_tracking")]
        public OpenTracking OpenTracking { get; set; }

        [JsonProperty(PropertyName = "subscription_tracking")]
        public SubscriptionTracking SubscriptionTracking { get; set; }

        [JsonProperty(PropertyName = "ganalytics")]
        public Ganalytics Ganalytics { get; set; }
    }


    public class BccSettings
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }


    public class BypassListManagement
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }
    }


    public class FooterSettings
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "html")]
        public string Html { get; set; }
    }


    public class SandboxMode
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }
    }


    public class SpamCheck
    {
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        [JsonProperty(PropertyName = "threshold")]
        public int? Threshold { get; set; }

        [JsonProperty(PropertyName = "post_to_url")]
        public string PostToUrl { get; set; }
    }


    public class MailSettings
    {
        [JsonProperty(PropertyName = "bcc")]
        public BccSettings BccSettings { get; set; }

        [JsonProperty(PropertyName = "bypass_list_management")]
        public BypassListManagement BypassListManagement { get; set; }

        [JsonProperty(PropertyName = "footer")]
        public FooterSettings FooterSettings { get; set; }

        [JsonProperty(PropertyName = "sandbox_mode")]
        public SandboxMode SandboxMode { get; set; }

        [JsonProperty(PropertyName = "spam_check")]
        public SpamCheck SpamCheck { get; set; }
    }


    public class Asm
    {
        [JsonProperty(PropertyName = "group_id")]
        public int? GroupId { get; set; }

        [JsonProperty(PropertyName = "groups_to_display")]
        public List<int> GroupsToDisplay { get; set; }
    }


    public class Attachment
    {
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        [JsonProperty(PropertyName = "disposition")]
        public string Disposition { get; set; }

        [JsonProperty(PropertyName = "content_id")]
        public string ContentId { get; set; }
    }


    public class Content
    {
        public Content()
        {
            return;
        }

        public Content(string type, string value)
        {
            Type = type;
            Value = value;
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }


    public class Email
    {
        public Email()
        {
            return;
        }

        public Email(string email, string name = null)
        {
            Address = email;
            Name = name;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Address { get; set; }
    }


    public class Personalization
    {
        [JsonProperty(PropertyName = "to")]
        public List<Email> Tos { get; set; }

        [JsonProperty(PropertyName = "cc")]
        public List<Email> Ccs { get; set; }

        [JsonProperty(PropertyName = "bcc")]
        public List<Email> Bccs { get; set; }

        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty(PropertyName = "substitutions")]
        public Dictionary<string, string> Substitutions { get; set; }

        [JsonProperty(PropertyName = "custom_args")]
        public Dictionary<string, string> CustomArgs { get; set; }

        [JsonProperty(PropertyName = "send_at")]
        public long? SendAt { get; set; }

        public void AddTo(Email email)
        {
            if (Tos == null)
            {
                Tos = new List<Email>();

            }
            Tos.Add(email);
        }

        public void AddCc(Email email)
        {
            if (Ccs == null)
            {
                Ccs = new List<Email>();
            }
            Ccs.Add(email);
        }

        public void AddBcc(Email email)
        {
            if (Bccs == null)
            {
                Bccs = new List<Email>();
            }
            Bccs.Add(email);
        }

        public void AddHeader(string key, string value)
        {
            if (Headers == null)
            {
                Headers = new Dictionary<string, string>();
            }
            Headers.Add(key, value);
        }

        public void AddSubstitution(string key, string value)
        {
            if (Substitutions == null)
            {
                Substitutions = new Dictionary<string, string>();
            }
            Substitutions.Add(key, value);
        }

        public void AddCustomArgs(string key, string value)
        {
            if (CustomArgs == null)
            {
                CustomArgs = new Dictionary<string, string>();
            }
            CustomArgs.Add(key, value);
        }
    }
}

